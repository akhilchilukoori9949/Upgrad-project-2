const uiService = (() => {

  var deptColors = {
    Engineering: 'dept-engineering',
    Marketing: 'dept-marketing',
    HR: 'dept-hr',
    Finance: 'dept-finance',
    Operations: 'dept-operations'
  };

  var avatarColors = ['#4361ee', '#3a86ff', '#7209b7', '#f72585', '#4cc9f0', '#2ec4b6', '#e76f51', '#264653'];

  function formatSalary(amount) {
    return 'Rs ' + Number(amount || 0).toLocaleString('en-IN');
  }

  function formatDate(dateStr) {
    if (!dateStr) return '';
    return new Date(dateStr).toLocaleDateString('en-IN', { day: '2-digit', month: 'short', year: 'numeric' });
  }

  function getInitials(firstName, lastName) {
    return ((firstName || '')[0] || '').toUpperCase() + ((lastName || '')[0] || '').toUpperCase();
  }

  function getAvatarColor(id) {
    return avatarColors[id % avatarColors.length];
  }

  function safe(str) {
    if (!str) return '';
    return String(str)
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;')
      .replace(/'/g, '&#39;');
  }

  function showTableLoading() {
    $('#employee-table-body').html(
      '<tr><td colspan="10" class="text-center py-5 text-muted">' +
        '<div class="spinner-border spinner-border-sm text-primary me-2" role="status"></div>' +
        'Loading employees...' +
      '</td></tr>'
    );
    $('#pagination-container').empty();
  }

  function renderEmployeeTable(pagedResult, isAdmin) {
    var $tbody = $('#employee-table-body');
    $tbody.empty();

    var employees = pagedResult && pagedResult.data ? pagedResult.data : [];
    if (!employees.length) {
      $tbody.html('<tr><td colspan="10" class="text-center py-5 text-muted"><i class="bi bi-search fs-2 d-block mb-2"></i>No employees found.</td></tr>');
      $('#emp-count-label').text('Showing 0 of 0 employees');
      renderPagination(null);
      return;
    }

    employees.forEach(function(emp, index) {
      var initials = getInitials(emp.firstName, emp.lastName);
      var deptClass = deptColors[emp.department] || 'dept-default';
      var statusClass = emp.status === 'Active' ? 'badge-active' : 'badge-inactive';
      var rowClass = index % 2 === 0 ? '' : 'row-alt';
      var actionButtons =
        '<button class="btn-action btn-view" data-id="' + emp.id + '" title="View"><i class="bi bi-eye"></i></button>' +
        (isAdmin ? '<button class="btn-action btn-edit" data-id="' + emp.id + '" title="Edit"><i class="bi bi-pencil"></i></button>' : '') +
        (isAdmin ? '<button class="btn-action btn-delete" data-id="' + emp.id + '" title="Delete"><i class="bi bi-trash3"></i></button>' : '');

      $tbody.append(
        '<tr class="' + rowClass + '" data-id="' + emp.id + '">' +
          '<td class="text-muted fw-semibold">#' + emp.id + '</td>' +
          '<td><div class="avatar-circle" style="background:' + getAvatarColor(emp.id) + '">' + initials + '</div></td>' +
          '<td class="fw-semibold">' + safe(emp.firstName) + ' ' + safe(emp.lastName) + '</td>' +
          '<td class="text-muted small">' + safe(emp.email) + '</td>' +
          '<td><span class="dept-badge ' + deptClass + '">' + safe(emp.department) + '</span></td>' +
          '<td>' + safe(emp.designation) + '</td>' +
          '<td class="fw-semibold text-primary">' + formatSalary(emp.salary) + '</td>' +
          '<td>' + formatDate(emp.joinDate) + '</td>' +
          '<td><span class="status-badge ' + statusClass + '">' + safe(emp.status) + '</span></td>' +
          '<td><div class="action-btns">' + actionButtons + '</div></td>' +
        '</tr>'
      );
    });

    var start = ((pagedResult.page - 1) * pagedResult.pageSize) + 1;
    var end = Math.min(start + employees.length - 1, pagedResult.totalCount);
    $('#emp-count-label').text('Showing ' + start + '-' + end + ' of ' + pagedResult.totalCount + ' employees');
    renderPagination(pagedResult);
  }

  function renderPagination(pagedResult) {
    var $container = $('#pagination-container');
    $container.empty();

    if (!pagedResult || !pagedResult.totalPages || pagedResult.totalPages <= 1) {
      return;
    }

    var html = '<nav aria-label="Employee pagination"><ul class="pagination justify-content-end mb-0">';
    html += '<li class="page-item' + (pagedResult.hasPrevPage ? '' : ' disabled') + '">' +
      '<button class="page-link pagination-link page-link-btn" data-page="' + (pagedResult.page - 1) + '"' + (pagedResult.hasPrevPage ? '' : ' disabled') + '>Prev</button></li>';

    for (var page = 1; page <= pagedResult.totalPages; page += 1) {
      html += '<li class="page-item' + (page === pagedResult.page ? ' active' : '') + '">' +
        '<button class="page-link pagination-link page-link-btn" data-page="' + page + '">' + page + '</button></li>';
    }

    html += '<li class="page-item' + (pagedResult.hasNextPage ? '' : ' disabled') + '">' +
      '<button class="page-link pagination-link page-link-btn" data-page="' + (pagedResult.page + 1) + '"' + (pagedResult.hasNextPage ? '' : ' disabled') + '>Next</button></li>';
    html += '</ul></nav>';

    $container.html(html);
  }

  function renderDashboardCards(summary) {
    $('#kpi-total').text(summary.totalEmployees || 0);
    $('#kpi-active').text(summary.activeEmployees || 0);
    $('#kpi-inactive').text(summary.inactiveEmployees || 0);
    $('#kpi-departments').text(summary.totalDepartments || 0);
  }

  function renderDepartmentBreakdown(items) {
    var $body = $('#dept-breakdown-body');
    $body.empty();

    var barColors = {
      Engineering: '#4361ee',
      Marketing: '#ffa600',
      HR: '#2ec4b6',
      Finance: '#2d6a4f',
      Operations: '#6c757d'
    };

    if (!items || !items.length) {
      $body.html('<tr><td colspan="4" class="text-center py-4 text-muted">No department data available.</td></tr>');
      return;
    }

    items.forEach(function(item) {
      var deptClass = deptColors[item.department] || 'dept-default';
      var pct = Number(item.percentage || 0);
      var barColor = barColors[item.department] || '#4361ee';

      $body.append(
        '<tr>' +
          '<td><span class="dept-badge ' + deptClass + '">' + safe(item.department) + '</span></td>' +
          '<td class="fw-semibold">' + item.count + '</td>' +
          '<td><div class="bar-track"><div class="bar-fill" style="width:' + pct + '%;background:' + barColor + '"></div></div></td>' +
          '<td class="text-muted small">' + pct + '%</td>' +
        '</tr>'
      );
    });
  }

  function renderRecentEmployees(employees) {
    var $list = $('#recent-employees-list');
    $list.empty();

    if (!employees || !employees.length) {
      $list.html('<p class="text-muted text-center py-3 small">No employees yet.</p>');
      return;
    }

    employees.forEach(function(emp) {
      var initials = getInitials(emp.firstName, emp.lastName);
      var deptClass = deptColors[emp.department] || 'dept-default';
      var statusClass = emp.status === 'Active' ? 'badge-active' : 'badge-inactive';

      $list.append(
        '<div class="recent-item">' +
          '<div class="avatar-circle avatar-sm" style="background:' + getAvatarColor(emp.id) + '">' + initials + '</div>' +
          '<div class="recent-info">' +
            '<div class="fw-semibold small">' + safe(emp.firstName) + ' ' + safe(emp.lastName) + '</div>' +
            '<div class="text-muted" style="font-size:0.75rem">' + safe(emp.designation) + '</div>' +
          '</div>' +
          '<div class="d-flex gap-1 flex-wrap justify-content-end">' +
            '<span class="dept-badge ' + deptClass + '">' + safe(emp.department) + '</span>' +
            '<span class="status-badge ' + statusClass + '">' + safe(emp.status) + '</span>' +
          '</div>' +
        '</div>'
      );
    });
  }

  function showModal(type, data) {
    if (type !== 'view') return;

    var employee = data;
    var initials = getInitials(employee.firstName, employee.lastName);
    var deptClass = deptColors[employee.department] || 'dept-default';
    var statusClass = employee.status === 'Active' ? 'badge-active' : 'badge-inactive';

    $('#view-avatar').text(initials).css('background', getAvatarColor(employee.id));
    $('#view-name').text(employee.firstName + ' ' + employee.lastName);
    $('#view-dept-badge').html('<span class="dept-badge ' + deptClass + '">' + safe(employee.department) + '</span>');
    $('#view-email').text(employee.email);
    $('#view-phone').text(employee.phone);
    $('#view-designation').text(employee.designation);
    $('#view-salary').text(formatSalary(employee.salary));
    $('#view-joinDate').text(formatDate(employee.joinDate));
    $('#view-status').html('<span class="status-badge ' + statusClass + '">' + safe(employee.status) + '</span>');

    bootstrap.Modal.getOrCreateInstance(document.getElementById('viewEmployeeModal')).show();
  }

  function populateForm(employee) {
    $('#emp-firstName').val(employee.firstName);
    $('#emp-lastName').val(employee.lastName);
    $('#emp-email').val(employee.email);
    $('#emp-phone').val(employee.phone);
    $('#emp-department').val(employee.department);
    $('#emp-designation').val(employee.designation);
    $('#emp-salary').val(employee.salary);
    $('#emp-joinDate').val((employee.joinDate || '').split('T')[0]);
    $('#emp-status').val(employee.status);
  }

  function clearForm() {
    $('#employee-form')[0].reset();
    clearFormErrors();
  }

  function showFormErrors(errors) {
    clearFormErrors();
    Object.keys(errors).forEach(function(field) {
      $('#emp-' + field).addClass('is-invalid');
      $('#emp-' + field + '-error').text(errors[field]).show();
    });
  }

  function clearFormErrors() {
    $('#employee-form .is-invalid').removeClass('is-invalid');
    $('#employee-form [data-error]').hide().text('');
  }

  function showAuthErrors(prefix, errors) {
    Object.keys(errors).forEach(function(field) {
      $('#' + prefix + '-' + field).addClass('is-invalid');
      $('#' + prefix + '-' + field + '-error').text(errors[field]).show();
    });
  }

  function clearAuthErrors(prefix) {
    $('#' + prefix + '-form .is-invalid').removeClass('is-invalid');
    $('#' + prefix + '-form [data-error]').hide().text('');
  }

  function setRoleBadge(role) {
    $('#nav-role-badge').text(role || 'Viewer');
  }

  function applyRoleUI(isAdmin, role) {
    $('[data-admin-only="true"]').toggle(!!isAdmin);
    $('#viewer-notice').toggle(!isAdmin);
    setRoleBadge(role || (isAdmin ? 'Admin' : 'Viewer'));
  }

  function showToast(message, type) {
    var $toast = $('#appToast');
    $toast.removeClass('toast-success toast-error toast-warning');

    if (type === 'success') {
      $toast.addClass('toast-success');
      $('#toast-icon').attr('class', 'bi bi-check-circle-fill fs-5');
    } else if (type === 'error') {
      $toast.addClass('toast-error');
      $('#toast-icon').attr('class', 'bi bi-x-circle-fill fs-5');
    } else {
      $toast.addClass('toast-warning');
      $('#toast-icon').attr('class', 'bi bi-exclamation-circle-fill fs-5');
    }

    $('#toast-message').text(message);
    bootstrap.Toast.getOrCreateInstance(document.getElementById('appToast'), { delay: 3500 }).show();
  }

  return {
    applyRoleUI,
    clearAuthErrors,
    clearForm,
    clearFormErrors,
    populateForm,
    renderDashboardCards,
    renderDepartmentBreakdown,
    renderEmployeeTable,
    renderRecentEmployees,
    setRoleBadge,
    showAuthErrors,
    showFormErrors,
    showModal,
    showTableLoading,
    showToast
  };

})();
