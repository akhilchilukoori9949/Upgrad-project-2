const storageService = (() => {

  function _headers(withAuth) {
    var headers = { 'Content-Type': 'application/json' };
    if (withAuth !== false) {
      var token = authService.getToken ? authService.getToken() : null;
      if (token) {
        headers.Authorization = 'Bearer ' + token;
      }
    }
    return headers;
  }

  async function _request(path, options, withAuth) {
    var response = await fetch(AppConfig.API_BASE_URL + path, Object.assign({
      headers: _headers(withAuth),
      method: 'GET'
    }, options || {}));

    var contentType = response.headers.get('content-type') || '';
    var payload = null;

    if (contentType.indexOf('application/json') !== -1) {
      payload = await response.json();
    } else {
      var text = await response.text();
      payload = text ? { message: text } : null;
    }

    if (!response.ok) {
      var error = new Error((payload && payload.message) || 'Request failed.');
      error.status = response.status;
      error.errors = payload && payload.errors ? payload.errors : null;
      error.payload = payload;
      throw error;
    }

    return payload;
  }

  function _toQueryString(query) {
    var params = new URLSearchParams();

    if (query.search) params.append('search', query.search);
    if (query.department && query.department !== 'All') params.append('department', query.department);
    if (query.status && query.status !== 'All') params.append('status', query.status);
    if (query.sortBy) params.append('sortBy', query.sortBy);
    if (query.sortDir) params.append('sortDir', query.sortDir);
    if (query.page) params.append('page', query.page);
    if (query.pageSize) params.append('pageSize', query.pageSize);

    var serialized = params.toString();
    return serialized ? '?' + serialized : '';
  }

  function getAll(query) {
    return _request('/employees' + _toQueryString(query || {}));
  }

  function getById(id) {
    return _request('/employees/' + id);
  }

  function add(employee) {
    return _request('/employees', {
      method: 'POST',
      body: JSON.stringify(employee)
    });
  }

  function update(id, data) {
    return _request('/employees/' + id, {
      method: 'PUT',
      body: JSON.stringify(data)
    });
  }

  function remove(id) {
    return _request('/employees/' + id, {
      method: 'DELETE'
    });
  }

  function getDashboard() {
    return _request('/employees/dashboard');
  }

  function login(credentials) {
    return _request('/auth/login', {
      method: 'POST',
      body: JSON.stringify(credentials)
    }, false);
  }

  function signup(credentials) {
    return _request('/auth/register', {
      method: 'POST',
      body: JSON.stringify(credentials)
    }, false);
  }

  return { getAll, getById, add, update, remove, getDashboard, login, signup };

})();
