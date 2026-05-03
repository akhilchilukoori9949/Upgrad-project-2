======================================================
  Employee Management System — Mini Project 1
======================================================

Name  : Chilukoori Akhil
Batch : .Net with Python-B2 (Prism Platform: .Net with Python-B6)


HOW TO RUN THE APP
------------------
1. Unzip the submitted ZIP folder

2. Open the file:
      employee-management-dashboard/index.html
   in any modern browser (Chrome, Firefox, or Edge)

3. That's it — no installation, no build step, no server needed.

Default login credentials:
   Username : admin
   Password : admin123

You can also create a new admin account using the Sign Up page.


HOW TO RUN THE TESTS
--------------------
Prerequisites: Node.js must be installed on your machine.
   Download from https://nodejs.org if not already installed.

Steps:

1. Open a terminal / command prompt

2. Navigate into the project folder:
      cd employee-management-dashboard

3. Install dependencies (only needed once):
      npm install

4. Run all tests:
      npm test

All 33 unit tests across the three test files should pass:
   - tests/employeeService.test.js   (add, update, remove, search, filter, sort)
   - tests/authService.test.js       (signup, login, session state)
   - tests/dashboardService.test.js  (summary, department breakdown, recent employees)


PROJECT STRUCTURE
-----------------
index.html               — Single HTML page containing all views:
                           Login, Signup, Dashboard, Employee List,
                           and all Bootstrap modals

css/styles.css           — Custom styles supplementing Bootstrap 5

js/data.js               — Static employee dataset (15 records) +
                           default admin credentials. Data only, no logic.
js/storageService.js     — In-memory data store. Single interface for all
                           data reads and writes (getAll, getById, add,
                           update, remove, nextId)
js/authService.js        — Authentication logic: signup, login, logout,
                           session tracking (isLoggedIn, getCurrentUser)
js/employeeService.js    — Employee business logic: search, filter by
                           department/status, applyFilters (AND logic), sortBy
js/validationService.js  — Form validation: employee form and auth form.
                           Returns field-level error objects. No DOM access.
js/dashboardService.js   — Dashboard computations: getSummary,
                           getDepartmentBreakdown, getRecentEmployees
js/uiService.js          — All DOM rendering and UI feedback: table, cards,
                           modals, toasts, inline errors
js/app.js                — Application entry point. Initialises the app and
                           wires all jQuery event listeners. No business logic.

tests/employeeService.test.js   — Unit tests for employee CRUD, search,
                                  filter, and sort logic
tests/authService.test.js       — Unit tests for signup, login, and
                                  session state
tests/dashboardService.test.js  — Unit tests for summary counts,
                                  department breakdown, recent employees

package.json             — Node project manifest. Jest declared as devDependency.
jest.config.js           — Jest configuration (testEnvironment: node,
                           testMatch: tests/**/*.test.js)
