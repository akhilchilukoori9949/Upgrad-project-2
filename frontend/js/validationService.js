const validationService = (() => {

  function validateEmployeeForm(formData) {
    var errors = {};

    if (!formData.firstName || formData.firstName.trim() === '') {
      errors.firstName = 'First name is required.';
    }

    if (!formData.lastName || formData.lastName.trim() === '') {
      errors.lastName = 'Last name is required.';
    }

    if (!formData.email || formData.email.trim() === '') {
      errors.email = 'Email address is required.';
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email.trim())) {
      errors.email = 'Please enter a valid email address.';
    }

    if (!formData.phone || formData.phone.trim() === '') {
      errors.phone = 'Phone number is required.';
    } else if (!/^\d{10}$/.test(formData.phone.trim())) {
      errors.phone = 'Phone number must be exactly 10 digits.';
    }

    if (!formData.department || formData.department === '') {
      errors.department = 'Please select a department.';
    }

    if (!formData.designation || formData.designation.trim() === '') {
      errors.designation = 'Designation is required.';
    }

    if (formData.salary === '' || formData.salary === null || formData.salary === undefined) {
      errors.salary = 'Salary is required.';
    } else if (isNaN(Number(formData.salary)) || Number(formData.salary) <= 0) {
      errors.salary = 'Salary must be a positive number.';
    }

    if (!formData.joinDate || formData.joinDate.trim() === '') {
      errors.joinDate = 'Join date is required.';
    }

    if (!formData.status || formData.status === '') {
      errors.status = 'Please select a status.';
    }

    return errors;
  }

  function validateAuthForm(formData) {
    var errors = {};

    if (!formData.username || formData.username.trim() === '') {
      errors.username = 'Username is required.';
    }

    if (!formData.password || formData.password === '') {
      errors.password = 'Password is required.';
    } else if (formData.isSignup && formData.password.length < 6) {
      errors.password = 'Password must be at least 6 characters.';
    }

    if (formData.isSignup) {
      if (!formData.confirmPassword || formData.confirmPassword === '') {
        errors.confirmPassword = 'Please confirm your password.';
      } else if (formData.password !== formData.confirmPassword) {
        errors.confirmPassword = 'Passwords do not match.';
      }
    }

    return errors;
  }

  function mapServerErrors(error) {
    var mapped = {};

    if (!error || !error.errors) {
      return mapped;
    }

    Object.keys(error.errors).forEach(function(key) {
      var normalizedKey = key.charAt(0).toLowerCase() + key.slice(1);
      var value = error.errors[key];
      mapped[normalizedKey] = Array.isArray(value) ? value[0] : value;
    });

    return mapped;
  }

  return { validateEmployeeForm, validateAuthForm, mapServerErrors };

})();
