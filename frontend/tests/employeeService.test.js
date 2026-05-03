let _store = [];

const storageService = {
  getAll: function() { return _store.map(function(e) { return Object.assign({}, e); }); },
  getById: function(id) {
    var e = _store.find(function(e) { return e.id === id; });
    return e ? Object.assign({}, e) : undefined;
  },
  add: function(emp) {
    var id = _store.length === 0 ? 1 : Math.max.apply(null, _store.map(function(e) { return e.id; })) + 1;
    var n = Object.assign({}, emp, { id: id });
    _store.push(n);
    return Object.assign({}, n);
  },
  update: function(id, d) {
    var i = _store.findIndex(function(e) { return e.id === id; });
    if (i === -1) return null;
    _store[i] = Object.assign({}, _store[i], d, { id: id });
    return Object.assign({}, _store[i]);
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
  function getById(id) { return storageService.getById(id); }
  function add(data) { return storageService.add(data); }
  function update(id, data) { return storageService.update(id, data); }
  function remove(id) { return storageService.remove(id); }

  function emailExists(email, excludeId) {
    excludeId = excludeId || null;
    return storageService.getAll().some(function(e) {
      return e.email.toLowerCase() === email.toLowerCase() && e.id !== excludeId;
    });
  }

  function search(query) {
    if (!query || query.trim() === '') return storageService.getAll();
    var q = query.trim().toLowerCase();
    return storageService.getAll().filter(function(e) {
      var full = (e.firstName + ' ' + e.lastName).toLowerCase();
      return full.includes(q) || e.email.toLowerCase().includes(q);
    });
  }

  function filterByDepartment(dept) {
    if (!dept || dept === 'All') return storageService.getAll();
    return storageService.getAll().filter(function(e) { return e.department === dept; });
  }

  function filterByStatus(status) {
    if (!status || status === 'All') return storageService.getAll();
    return storageService.getAll().filter(function(e) { return e.status === status; });
  }

  function applyFilters(searchQuery, dept, status) {
    var results = storageService.getAll();
    if (searchQuery && searchQuery.trim() !== '') {
      var q = searchQuery.trim().toLowerCase();
      results = results.filter(function(e) {
        var full = (e.firstName + ' ' + e.lastName).toLowerCase();
        return full.includes(q) || e.email.toLowerCase().includes(q);
      });
    }
    if (dept && dept !== 'All') results = results.filter(function(e) { return e.department === dept; });
    if (status && status !== 'All') results = results.filter(function(e) { return e.status === status; });
    return results;
  }

  function sortBy(employees, field, direction) {
    var sorted = employees.slice();
    sorted.sort(function(a, b) {
      if (field === 'name') {
        var va = a.lastName.toLowerCase(), vb = b.lastName.toLowerCase();
        if (va < vb) return direction === 'asc' ? -1 : 1;
        if (va > vb) return direction === 'asc' ? 1 : -1;
        return 0;
      }
      if (field === 'salary') return direction === 'asc' ? a.salary - b.salary : b.salary - a.salary;
      if (field === 'joinDate') return direction === 'asc'
        ? new Date(a.joinDate) - new Date(b.joinDate)
        : new Date(b.joinDate) - new Date(a.joinDate);
      return 0;
    });
    return sorted;
  }

  return { getAll, getById, add, update, remove, emailExists, search, filterByDepartment, filterByStatus, applyFilters, sortBy };
})();

const mockEmployees = [
  { id: 1, firstName: 'Priya',  lastName: 'Prabhu', email: 'priya@test.com', phone: '9876543210', department: 'Engineering', designation: 'Software Engineer', salary: 850000,  joinDate: '2021-03-15', status: 'Active'   },
  { id: 2, firstName: 'Arjun',  lastName: 'Sharma', email: 'arjun@test.com', phone: '9812345678', department: 'Marketing',   designation: 'Marketing Exec',    salary: 620000,  joinDate: '2020-07-01', status: 'Active'   },
  { id: 3, firstName: 'Neha',   lastName: 'Kapoor', email: 'neha@test.com',  phone: '9823456789', department: 'HR',          designation: 'HR Executive',      salary: 550000,  joinDate: '2019-11-20', status: 'Inactive' },
  { id: 4, firstName: 'Rahul',  lastName: 'Verma',  email: 'rahul@test.com', phone: '9834567890', department: 'Engineering', designation: 'Senior Developer',  salary: 1000000, joinDate: '2022-01-10', status: 'Active'   }
];

beforeEach(function() { storageService._reset(mockEmployees); });

describe('add', function() {
  test('assigns next incremented id to new employee', function() {
    var result = employeeService.add({ firstName: 'Test', lastName: 'User', email: 'test@test.com', phone: '9999999999', department: 'Finance', designation: 'Analyst', salary: 500000, joinDate: '2024-01-01', status: 'Active' });
    expect(result.id).toBe(5);
    expect(employeeService.getAll().length).toBe(5);
  });

  test('total count increases by one after add', function() {
    var before = employeeService.getAll().length;
    employeeService.add({ firstName: 'X', lastName: 'Y', email: 'xy@test.com', phone: '1234567890', department: 'HR', designation: 'Exec', salary: 400000, joinDate: '2024-06-01', status: 'Active' });
    expect(employeeService.getAll().length).toBe(before + 1);
  });

  test('added employee data is stored correctly', function() {
    employeeService.add({ firstName: 'Sita', lastName: 'Ram', email: 'sita@test.com', phone: '9111111111', department: 'Finance', designation: 'CA', salary: 700000, joinDate: '2024-02-01', status: 'Active' });
    var emp = employeeService.getAll().find(function(e) { return e.email === 'sita@test.com'; });
    expect(emp.firstName).toBe('Sita');
  });
});

describe('update', function() {
  test('updates the correct field by id', function() {
    employeeService.update(1, { salary: 999999 });
    expect(employeeService.getById(1).salary).toBe(999999);
  });

  test('returns null for non-existent id', function() {
    expect(employeeService.update(999, { salary: 0 })).toBeNull();
  });

  test('other fields remain unchanged after partial update', function() {
    employeeService.update(1, { designation: 'Lead Engineer' });
    var emp = employeeService.getById(1);
    expect(emp.firstName).toBe('Priya');
    expect(emp.designation).toBe('Lead Engineer');
  });
});

describe('remove', function() {
  test('removes employee and reduces count', function() {
    employeeService.remove(2);
    expect(employeeService.getAll().length).toBe(3);
    expect(employeeService.getById(2)).toBeUndefined();
  });

  test('returns false for non-existent id', function() {
    expect(employeeService.remove(999)).toBe(false);
  });

  test('remaining record ids are not affected', function() {
    employeeService.remove(2);
    expect(employeeService.getById(1).id).toBe(1);
    expect(employeeService.getById(3).id).toBe(3);
  });
});

describe('search', function() {
  test('finds employee by first name case-insensitively', function() {
    expect(employeeService.search('PRIYA')[0].firstName).toBe('Priya');
  });

  test('finds employee by email', function() {
    expect(employeeService.search('arjun@test.com').length).toBe(1);
  });

  test('returns empty array when no match', function() {
    expect(employeeService.search('zzznomatch').length).toBe(0);
  });

  test('returns all employees for empty query', function() {
    expect(employeeService.search('').length).toBe(4);
  });
});

describe('filterByDepartment', function() {
  test('returns only employees in the given department', function() {
    var results = employeeService.filterByDepartment('Engineering');
    expect(results.length).toBe(2);
    results.forEach(function(e) { expect(e.department).toBe('Engineering'); });
  });

  test('returns all employees when dept is All', function() {
    expect(employeeService.filterByDepartment('All').length).toBe(4);
  });
});

describe('filterByStatus', function() {
  test('returns only Active employees', function() {
    expect(employeeService.filterByStatus('Active').every(function(e) { return e.status === 'Active'; })).toBe(true);
  });

  test('returns only Inactive employees', function() {
    expect(employeeService.filterByStatus('Inactive').every(function(e) { return e.status === 'Inactive'; })).toBe(true);
  });
});

describe('applyFilters', function() {
  test('all three filters work together with AND logic', function() {
    var results = employeeService.applyFilters('priya', 'Engineering', 'Active');
    expect(results.length).toBe(1);
    expect(results[0].firstName).toBe('Priya');
  });

  test('returns empty when filters match nothing', function() {
    expect(employeeService.applyFilters('priya', 'HR', 'Active').length).toBe(0);
  });

  test('returns all when all filters are reset', function() {
    expect(employeeService.applyFilters('', 'All', 'All').length).toBe(4);
  });
});

describe('sortBy', function() {
  test('salary ascending puts lowest first', function() {
    var sorted = employeeService.sortBy(employeeService.getAll(), 'salary', 'asc');
    expect(sorted[0].salary).toBeLessThanOrEqual(sorted[sorted.length - 1].salary);
  });

  test('salary descending puts highest first', function() {
    var sorted = employeeService.sortBy(employeeService.getAll(), 'salary', 'desc');
    expect(sorted[0].salary).toBeGreaterThanOrEqual(sorted[sorted.length - 1].salary);
  });

  test('name ascending sorts last names A to Z', function() {
    var sorted = employeeService.sortBy(employeeService.getAll(), 'name', 'asc');
    var names = sorted.map(function(e) { return e.lastName.toLowerCase(); });
    expect(names).toEqual(names.slice().sort());
  });

  test('joinDate descending puts newest first', function() {
    var sorted = employeeService.sortBy(employeeService.getAll(), 'joinDate', 'desc');
    expect(new Date(sorted[0].joinDate).getTime()).toBeGreaterThanOrEqual(new Date(sorted[1].joinDate).getTime());
  });
});
