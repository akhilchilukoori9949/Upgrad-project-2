const authService = (() => {

  var _session = null;

  function _decodeToken(token) {
    if (!token) return {};

    try {
      var payload = token.split('.')[1];
      var normalized = payload.replace(/-/g, '+').replace(/_/g, '/');
      var json = decodeURIComponent(atob(normalized).split('').map(function(ch) {
        return '%' + ('00' + ch.charCodeAt(0).toString(16)).slice(-2);
      }).join(''));
      return JSON.parse(json);
    } catch (error) {
      return {};
    }
  }

  function _resolveRole(result) {
    var decoded = _decodeToken(result.token);
    return decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || decoded.role || result.role || 'Viewer';
  }

  async function signup(username, password, role) {
    try {
      var result = await storageService.signup({
        username: username,
        password: password,
        role: role || 'Viewer'
      });

      return {
        success: true,
        error: null,
        username: result.username,
        role: _resolveRole(result)
      };
    } catch (error) {
      return {
        success: false,
        error: error.message || 'Unable to register.',
        fieldErrors: error.errors || null
      };
    }
  }

  async function login(username, password) {
    try {
      var result = await storageService.login({
        username: username,
        password: password
      });

      _session = {
        username: result.username,
        role: _resolveRole(result),
        token: result.token
      };

      return {
        success: true,
        error: null,
        username: _session.username,
        role: _session.role
      };
    } catch (error) {
      _session = null;
      return {
        success: false,
        error: error.message || 'Invalid credentials.',
        fieldErrors: error.errors || null
      };
    }
  }

  function logout() {
    _session = null;
  }

  function isLoggedIn() {
    return !!(_session && _session.token);
  }

  function getCurrentUser() {
    return _session ? _session.username : null;
  }

  function getCurrentRole() {
    return _session ? _session.role : null;
  }

  function isAdmin() {
    return getCurrentRole() === 'Admin';
  }

  function getToken() {
    return _session ? _session.token : null;
  }

  return { signup, login, logout, isLoggedIn, getCurrentUser, getCurrentRole, isAdmin, getToken };

})();
