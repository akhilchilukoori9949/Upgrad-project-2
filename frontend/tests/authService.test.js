const authService = (() => {
  var admins = [];
  var currentUser = null;
  var loggedIn = false;

  function signup(username, password) {
    var exists = admins.find(function(a) {
      return a.username.toLowerCase() === username.toLowerCase();
    });
    if (exists) return { success: false, error: 'Username already exists. Please choose a different one.' };
    admins.push({ username: username, password: password });
    return { success: true, error: null };
  }

  function login(username, password) {
    var admin = admins.find(function(a) {
      return a.username.toLowerCase() === username.toLowerCase() && a.password === password;
    });
    if (!admin) return { success: false, error: 'Invalid credentials. Please check your username and password.' };
    currentUser = admin.username;
    loggedIn = true;
    return { success: true, error: null };
  }

  function logout() { currentUser = null; loggedIn = false; }
  function isLoggedIn() { return loggedIn; }
  function getCurrentUser() { return currentUser; }

  function _reset(adminData) {
    admins = adminData.map(function(a) { return Object.assign({}, a); });
    currentUser = null;
    loggedIn = false;
  }

  return { signup, login, logout, isLoggedIn, getCurrentUser, _reset };
})();

beforeEach(function() { authService._reset([{ username: 'admin', password: 'admin123' }]); });

describe('signup', function() {
  test('new unique user is registered successfully', function() {
    var result = authService.signup('newuser', 'password123');
    expect(result.success).toBe(true);
    expect(result.error).toBeNull();
  });

  test('duplicate username is rejected', function() {
    var result = authService.signup('admin', 'somepassword');
    expect(result.success).toBe(false);
    expect(result.error).toContain('already exists');
  });

  test('duplicate check is case insensitive', function() {
    expect(authService.signup('ADMIN', 'pass').success).toBe(false);
  });

  test('two different users can both register', function() {
    authService.signup('user1', 'pass1');
    expect(authService.signup('user2', 'pass2').success).toBe(true);
  });
});

describe('login', function() {
  test('correct credentials succeed', function() {
    expect(authService.login('admin', 'admin123').success).toBe(true);
  });

  test('wrong password fails', function() {
    expect(authService.login('admin', 'wrongpass').success).toBe(false);
  });

  test('unknown username fails', function() {
    expect(authService.login('nobody', 'admin123').success).toBe(false);
  });

  test('empty credentials fail', function() {
    expect(authService.login('', '').success).toBe(false);
  });
});

describe('session state', function() {
  test('not logged in before login', function() {
    expect(authService.isLoggedIn()).toBe(false);
  });

  test('logged in after successful login', function() {
    authService.login('admin', 'admin123');
    expect(authService.isLoggedIn()).toBe(true);
  });

  test('getCurrentUser returns correct username', function() {
    authService.login('admin', 'admin123');
    expect(authService.getCurrentUser()).toBe('admin');
  });

  test('logout clears the session', function() {
    authService.login('admin', 'admin123');
    authService.logout();
    expect(authService.isLoggedIn()).toBe(false);
    expect(authService.getCurrentUser()).toBeNull();
  });

  test('newly signed up user can log in', function() {
    authService.signup('newadmin', 'securepass');
    expect(authService.login('newadmin', 'securepass').success).toBe(true);
    expect(authService.getCurrentUser()).toBe('newadmin');
  });
});
