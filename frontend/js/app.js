$(document).ready(function() {

  var state = {
    search: '',
    department: 'All',
    status: 'All',
    sortBy: 'name',
    sortDir: 'asc',
    page: 1,
    pageSize: AppConfig.PAGE_SIZE,
    editingEmployeeId: null
  };

  var searchTimer = null;

  function init() {
    showView('login');
    uiService.applyRoleUI(false, 'Viewer');
    updateSortIndicators();
  }

  function showView(view) {
    $('#view-login, #view-signup, #view-app').hide();
    $('#main-navbar').css('display', 'none');

    if (view === 'login') {
      $('#view-login').show();
    } else if (view === 'signup') {
      $('#view-signup').show();
    }
  }

  async function showApp(section) {
    $('#view-login, #view-signup').hide();
    $('#view-app').show();
    $('#main-navbar').css('display', '');
    $('#nav-username').text(authService.getCurrentUser() || 'User');
    uiService.applyRoleUI(authService.isAdmin(), authService.getCurrentRole());
    await showSection(section);
  }

  async function showSection(section) {
    $('#section-dashboard, #section-employees').hide();
    $('#nav-dashboard, #nav-employees').removeClass('active');

    if (section === 'dashboard') {
      $('#section-dashboard').show();
      $('#nav-dashboard').addClass('active');
      await refreshDashboard();
    } else if (section === 'employees') {
      $('#section-employees').show();
      $('#nav-employees').addClass('active');
      uiService.applyRoleUI(authService.isAdmin(), authService.getCurrentRole());
      await refreshEmployeeTable();
    }
  }

  async function refreshDashboard() {
    try {
      var summary = await dashboardService.getSummary();
      uiService.renderDashboardCards(summary);
      uiService.renderDepartmentBreakdown(summary.departmentBreakdown || []);
      uiService.renderRecentEmployees(summary.recentEmployees || []);
    } catch (error) {
      handleApiError(error, 'Unable to load dashboard.');
    }
  }

  async function refreshEmployeeTable() {
    uiService.showTableLoading();

    try {
      var result = await employeeService.getAll({
        search: state.search,
        department: state.department,
        status: state.status,
        sortBy: state.sortBy,
        sortDir: state.sortDir,
        page: state.page,
        pageSize: state.pageSize
      });

      uiService.renderEmployeeTable(result, authService.isAdmin());
      updateSortIndicators();
      return result;
    } catch (error) {
      handleApiError(error, 'Unable to load employees.');
      return null;
    }
  }

  function updateSortIndicators() {
    $('th.sortable .sort-icon').text('<>');
    $('th[data-sort="' + state.sortBy + '"] .sort-icon').text(state.sortDir === 'asc' ? '^' : 'v');
  }

  function openEmployeeModal(employee) {
    if (!authService.isAdmin()) {
      uiService.showToast('You have read-only access for this action.', 'warning');
      return;
    }

    uiService.clearForm();

    if (employee) {
      state.editingEmployeeId = employee.id;
      uiService.populateForm(employee);
      $('#emp-modal-title').text('Edit Employee');
      $('#emp-submit-btn').text('Update Employee');
    } else {
      state.editingEmployeeId = null;
      $('#emp-modal-title').text('Add Employee');
      $('#emp-submit-btn').text('Save Employee');
    }

    bootstrap.Modal.getOrCreateInstance(document.getElementById('employeeModal')).show();
  }

  function resetToLogin(message) {
    authService.logout();
    showView('login');
    $('#login-form')[0].reset();
    $('#login-error-msg').text(message || 'Please log in.').show();
  }

  function handleApiError(error, fallbackMessage) {
    if (error && error.status === 401) {
      resetToLogin('Session expired. Please log in again.');
      uiService.showToast('Session expired. Please log in again.', 'warning');
      return;
    }

    if (error && error.status === 403) {
      uiService.showToast('You have read-only access for this action.', 'warning');
      return;
    }

    uiService.showToast((error && error.message) || fallbackMessage || 'Something went wrong.', 'error');
  }

  $('#login-form').on('submit', async function(e) {
    e.preventDefault();
    uiService.clearAuthErrors('login');
    $('#login-error-msg').hide();

    var username = $('#login-username').val().trim();
    var password = $('#login-password').val();
    var errors = validationService.validateAuthForm({ username: username, password: password, isSignup: false });

    if (Object.keys(errors).length > 0) {
      uiService.showAuthErrors('login', errors);
      return;
    }

    var result = await authService.login(username, password);
    if (!result.success) {
      $('#login-error-msg').text(result.error).show();
      return;
    }

    uiService.showToast('Welcome back, ' + authService.getCurrentUser() + '!', 'success');
    await showApp('dashboard');
  });

  $('#signup-form').on('submit', async function(e) {
    e.preventDefault();
    uiService.clearAuthErrors('signup');

    var username = $('#signup-username').val().trim();
    var password = $('#signup-password').val();
    var confirmPassword = $('#signup-confirmPassword').val();
    var errors = validationService.validateAuthForm({
      username: username,
      password: password,
      confirmPassword: confirmPassword,
      isSignup: true
    });

    if (Object.keys(errors).length > 0) {
      uiService.showAuthErrors('signup', errors);
      return;
    }

    var result = await authService.signup(username, password, 'Viewer');
    if (!result.success) {
      if (result.fieldErrors) {
        uiService.showAuthErrors('signup', validationService.mapServerErrors({ errors: result.fieldErrors }));
      } else {
        uiService.showAuthErrors('signup', { username: result.error });
      }
      return;
    }

    uiService.showToast('Account created. Please log in.', 'success');
    $('#signup-form')[0].reset();
    showView('login');
  });

  $('#go-to-signup').on('click', function(e) {
    e.preventDefault();
    uiService.clearAuthErrors('login');
    $('#login-form')[0].reset();
    $('#login-error-msg').hide();
    showView('signup');
  });

  $('#go-to-login').on('click', function(e) {
    e.preventDefault();
    uiService.clearAuthErrors('signup');
    $('#signup-form')[0].reset();
    showView('login');
  });

  $('#logout-btn').on('click', function() {
    authService.logout();
    resetToLogin('You have been logged out.');
    uiService.showToast('You have been logged out.', 'success');
  });

  $('#nav-dashboard').on('click', async function(e) {
    e.preventDefault();
    if (!authService.isLoggedIn()) {
      showView('login');
      return;
    }

    await showSection('dashboard');
  });

  $('#nav-employees').on('click', async function(e) {
    e.preventDefault();
    if (!authService.isLoggedIn()) {
      showView('login');
      return;
    }

    await showSection('employees');
  });

  $(document).on('click', '.open-add-modal-btn', async function() {
    if (!authService.isLoggedIn()) {
      showView('login');
      return;
    }

    if ($('#section-employees').is(':hidden')) {
      await showSection('employees');
    }

    openEmployeeModal(null);
  });

  $('#search-input').on('input', function() {
    state.search = $(this).val().trim();
    state.page = 1;

    clearTimeout(searchTimer);
    searchTimer = setTimeout(function() {
      refreshEmployeeTable();
    }, AppConfig.SEARCH_DEBOUNCE_MS);
  });

  $('#dept-filter').on('change', function() {
    state.department = $(this).val();
    state.page = 1;
    refreshEmployeeTable();
  });

  $(document).on('click', '.status-filter-btn', function() {
    $('.status-filter-btn').removeClass('active');
    $(this).addClass('active');
    state.status = $(this).data('status');
    state.page = 1;
    refreshEmployeeTable();
  });

  $(document).on('click', 'th.sortable', function() {
    var field = $(this).data('sort');
    if (state.sortBy === field) {
      state.sortDir = state.sortDir === 'asc' ? 'desc' : 'asc';
    } else {
      state.sortBy = field;
      state.sortDir = 'asc';
    }

    state.page = 1;
    refreshEmployeeTable();
  });

  $(document).on('click', '.page-link-btn', function() {
    var nextPage = Number($(this).data('page'));
    if (!nextPage || $(this).is(':disabled') || nextPage === state.page) {
      return;
    }

    state.page = nextPage;
    refreshEmployeeTable();
  });

  $(document).on('click', '.btn-view', async function() {
    try {
      var employee = await employeeService.getById(parseInt($(this).data('id'), 10));
      uiService.showModal('view', employee);
    } catch (error) {
      handleApiError(error, 'Unable to load employee details.');
    }
  });

  $(document).on('click', '.btn-edit', async function() {
    try {
      var employee = await employeeService.getById(parseInt($(this).data('id'), 10));
      openEmployeeModal(employee);
    } catch (error) {
      handleApiError(error, 'Unable to load employee details.');
    }
  });

  $(document).on('click', '.btn-delete', async function() {
    try {
      var employee = await employeeService.getById(parseInt($(this).data('id'), 10));
      $('#delete-emp-name').text(employee.firstName + ' ' + employee.lastName);
      $('#confirm-delete-btn').data('id', employee.id);
      bootstrap.Modal.getOrCreateInstance(document.getElementById('deleteModal')).show();
    } catch (error) {
      handleApiError(error, 'Unable to load employee details.');
    }
  });

  $('#confirm-delete-btn').on('click', async function() {
    var id = parseInt($(this).data('id'), 10);
    var employeeName = $('#delete-emp-name').text() || 'Employee';

    try {
      await employeeService.remove(id);
      bootstrap.Modal.getOrCreateInstance(document.getElementById('deleteModal')).hide();
      uiService.showToast(employeeName + ' has been deleted.', 'success');

      var result = await refreshEmployeeTable();
      if (result && result.data && result.data.length === 0 && state.page > 1) {
        state.page -= 1;
        await refreshEmployeeTable();
      }

      await refreshDashboard();
    } catch (error) {
      handleApiError(error, 'Unable to delete employee.');
    }
  });

  $('#employee-form').on('submit', async function(e) {
    e.preventDefault();
    uiService.clearFormErrors();

    var formData = {
      firstName: $('#emp-firstName').val().trim(),
      lastName: $('#emp-lastName').val().trim(),
      email: $('#emp-email').val().trim(),
      phone: $('#emp-phone').val().trim(),
      department: $('#emp-department').val(),
      designation: $('#emp-designation').val().trim(),
      salary: $('#emp-salary').val(),
      joinDate: $('#emp-joinDate').val(),
      status: $('#emp-status').val()
    };

    var errors = validationService.validateEmployeeForm(formData);
    if (Object.keys(errors).length > 0) {
      uiService.showFormErrors(errors);
      return;
    }

    var payload = Object.assign({}, formData, { salary: Number(formData.salary) });

    try {
      if (state.editingEmployeeId) {
        await employeeService.update(state.editingEmployeeId, payload);
        uiService.showToast(formData.firstName + ' ' + formData.lastName + ' updated successfully.', 'success');
      } else {
        state.page = 1;
        await employeeService.add(payload);
        uiService.showToast(formData.firstName + ' ' + formData.lastName + ' added successfully.', 'success');
      }

      bootstrap.Modal.getOrCreateInstance(document.getElementById('employeeModal')).hide();
      uiService.clearForm();
      state.editingEmployeeId = null;
      await refreshEmployeeTable();
      await refreshDashboard();
    } catch (error) {
      var serverErrors = validationService.mapServerErrors(error);
      if (Object.keys(serverErrors).length > 0) {
        uiService.showFormErrors(serverErrors);
        return;
      }

      handleApiError(error, 'Unable to save employee.');
    }
  });

  document.getElementById('employeeModal').addEventListener('hidden.bs.modal', function() {
    uiService.clearForm();
    state.editingEmployeeId = null;
  });

  $('#login-username, #login-password').on('input', function() {
    $('#login-error-msg').hide();
  });

  init();

});
