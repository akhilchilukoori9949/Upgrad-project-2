let _store = [];

const storageService = {
  getAll: function() { return _store.map(function(e) { return Object.assign({}, e); }); },
  add: function(emp) {
    var id = _store.length === 0 ? 1 : Math.max.apply(null, _store.map(function(e) { return e.id; })) + 1;
    var n = Object.assign({}, emp, { id: id });
    _store.push(n);
    return Object.assign({}, n);
  },
  remove: function(id) {
    var i = _store.findIndex(function(e) { return e.id === id; });
    if (i === -1) return false;
    _store.splice(i, 1);
    return true;
  },
  _reset: function(data) { _store = data.map(function(e) { return Object.assign({}, e); }); }
};

const employeeService = (() => {
  function getAll() { return storageService.getAll(); }
  return { getAll };
})();

const dashboardService = (() => {
  function getSummary() {
    var all = employeeService.getAll();
    var departments = [];
    all.forEach(function(e) {
      if (departments.indexOf(e.department) === -1) departments.push(e.department);
    });
    return {
      total: all.length,
      active: all.filter(function(e) { return e.status === 'Active'; }).length,
      inactive: all.filter(function(e) { return e.status === 'Inactive'; }).length,
      departments: departments.length
    };
  }

  function getDepartmentBreakdown() {
    var all = employeeService.getAll();
    var breakdown = {};
    all.forEach(function(e) {
      breakdown[e.department] = (breakdown[e.department] || 0) + 1;
    });
    return breakdown;
  }

  function getRecentEmployees(n) {
    n = n || 5;
    return employeeService.getAll()
      .sort(function(a, b) { return b.id - a.id; })
      .slice(0, n);
  }

  return { getSummary, getDepartmentBreakdown, getRecentEmployees };
})();

const mockEmployees = [
  { id: 1, firstName: 'Priya',  lastName: 'Prabhu', email: 'p1@t.com', phone: '9000000001', department: 'Engineering', designation: 'SE',  salary: 850000,  joinDate: '2021-03-15', status: 'Active'   },
  { id: 2, firstName: 'Arjun',  lastName: 'Sharma', email: 'p2@t.com', phone: '9000000002', department: 'Marketing',   designation: 'ME',  salary: 620000,  joinDate: '2020-07-01', status: 'Active'   },
  { id: 3, firstName: 'Neha',   lastName: 'Kapoor', email: 'p3@t.com', phone: '9000000003', department: 'HR',          designation: 'HRE', salary: 550000,  joinDate: '2019-11-20', status: 'Inactive' },
  { id: 4, firstName: 'Rahul',  lastName: 'Verma',  email: 'p4@t.com', phone: '9000000004', department: 'Engineering', designation: 'Dev', salary: 1000000, joinDate: '2022-01-10', status: 'Active'   },
  { id: 5, firstName: 'Sneha',  lastName: 'Prasad', email: 'p5@t.com', phone: '9000000005', department: 'Finance',     designation: 'Fin', salary: 700000,  joinDate: '2023-06-01', status: 'Inactive' },
  { id: 6, firstName: 'Vikram', lastName: 'Raj',    email: 'p6@t.com', phone: '9000000006', department: 'Operations',  designation: 'Ops', salary: 800000,  joinDate: '2024-02-15', status: 'Active'   }
];

beforeEach(function() { storageService._reset(mockEmployees); });

describe('getSummary', function() {
  test('total count is correct', function() {
    expect(dashboardService.getSummary().total).toBe(6);
  });

  test('active count is correct', function() {
    expect(dashboardService.getSummary().active).toBe(4);
  });

  test('inactive count is correct', function() {
    expect(dashboardService.getSummary().inactive).toBe(2);
  });

  test('department count is correct', function() {
    expect(dashboardService.getSummary().departments).toBe(5);
  });

  test('active and inactive add up to total', function() {
    var s = dashboardService.getSummary();
    expect(s.active + s.inactive).toBe(s.total);
  });

  test('total updates after adding an employee', function() {
    storageService.add({ firstName: 'X', lastName: 'Y', email: 'xy@t.com', phone: '9111111111', department: 'HR', designation: 'Exec', salary: 400000, joinDate: '2024-01-01', status: 'Active' });
    expect(dashboardService.getSummary().total).toBe(7);
  });
});

describe('getDepartmentBreakdown', function() {
  test('Engineering has 2 employees', function() {
    expect(dashboardService.getDepartmentBreakdown()['Engineering']).toBe(2);
  });

  test('all other departments have 1 employee each', function() {
    var bd = dashboardService.getDepartmentBreakdown();
    expect(bd['Marketing']).toBe(1);
    expect(bd['HR']).toBe(1);
    expect(bd['Finance']).toBe(1);
    expect(bd['Operations']).toBe(1);
  });

  test('total across all departments matches total employees', function() {
    var bd = dashboardService.getDepartmentBreakdown();
    var total = Object.values(bd).reduce(function(a, b) { return a + b; }, 0);
    expect(total).toBe(6);
  });

  test('count updates after removing an employee', function() {
    storageService.remove(1);
    expect(dashboardService.getDepartmentBreakdown()['Engineering']).toBe(1);
  });
});

describe('getRecentEmployees', function() {
  test('returns employees ordered by highest id first', function() {
    var recent = dashboardService.getRecentEmployees(3);
    expect(recent[0].id).toBe(6);
    expect(recent[1].id).toBe(5);
    expect(recent[2].id).toBe(4);
  });

  test('returns exactly n employees', function() {
    expect(dashboardService.getRecentEmployees(3).length).toBe(3);
  });

  test('returns all when n is larger than total', function() {
    expect(dashboardService.getRecentEmployees(100).length).toBe(6);
  });

  test('newly added employee appears at top', function() {
    var added = storageService.add({ firstName: 'New', lastName: 'Person', email: 'new@t.com', phone: '9222222222', department: 'Marketing', designation: 'Designer', salary: 600000, joinDate: '2024-03-01', status: 'Active' });
    expect(dashboardService.getRecentEmployees(1)[0].id).toBe(added.id);
  });
});
