const initialEmployees = [
  { id: 1,  firstName: 'Priya',    lastName: 'Prabhu',    email: 'priya.prabhu@xyz.com',    phone: '9876543210', department: 'Engineering', designation: 'Software Engineer',     salary: 850000,  joinDate: '2021-03-15', status: 'Active'   },
  { id: 2,  firstName: 'Arjun',   lastName: 'Sharma',    email: 'arjun.sharma@xyz.com',    phone: '9812345678', department: 'Marketing',   designation: 'Marketing Executive',  salary: 620000,  joinDate: '2020-07-01', status: 'Active'   },
  { id: 3,  firstName: 'Neha',    lastName: 'Kapoor',    email: 'neha.kapoor@xyz.com',     phone: '9823456789', department: 'HR',          designation: 'HR Executive',         salary: 550000,  joinDate: '2019-11-20', status: 'Active'   },
  { id: 4,  firstName: 'Rahul',   lastName: 'Verma',     email: 'rahul.verma@xyz.com',     phone: '9834567890', department: 'Finance',     designation: 'Financial Analyst',    salary: 720000,  joinDate: '2022-01-10', status: 'Active'   },
  { id: 5,  firstName: 'Sneha',   lastName: 'Prasad',    email: 'sneha.prasad@xyz.com',    phone: '9845678901', department: 'Operations',  designation: 'Operations Manager',   salary: 950000,  joinDate: '2018-06-05', status: 'Active'   },
  { id: 6,  firstName: 'Vikram',  lastName: 'Raj',       email: 'vikram.raj@xyz.com',      phone: '9856789012', department: 'Engineering', designation: 'Senior Developer',     salary: 1100000, joinDate: '2017-09-12', status: 'Active'   },
  { id: 7,  firstName: 'Ananya',  lastName: 'Singh',     email: 'ananya.singh@xyz.com',    phone: '9867890123', department: 'Marketing',   designation: 'Content Strategist',   salary: 580000,  joinDate: '2023-02-28', status: 'Inactive' },
  { id: 8,  firstName: 'Karthik', lastName: 'Rajan',     email: 'karthik.rajan@xyz.com',   phone: '9878901234', department: 'Finance',     designation: 'Accounts Manager',     salary: 800000,  joinDate: '2020-04-17', status: 'Active'   },
  { id: 9,  firstName: 'Divya',   lastName: 'Kumar',     email: 'divya.kumar@xyz.com',     phone: '9889012345', department: 'HR',          designation: 'Talent Acquisition',   salary: 690000,  joinDate: '2021-08-22', status: 'Active'   },
  { id: 10, firstName: 'Rohan',   lastName: 'Mehta',     email: 'rohan.mehta@xyz.com',     phone: '9890123456', department: 'Engineering', designation: 'DevOps Engineer',      salary: 920000,  joinDate: '2022-05-03', status: 'Active'   },
  { id: 11, firstName: 'Lakshmi', lastName: 'Chandran',  email: 'lakshmi.chandran@xyz.com',phone: '9801234567', department: 'Marketing',   designation: 'Brand Manager',        salary: 750000,  joinDate: '2021-12-11', status: 'Active'   },
  { id: 12, firstName: 'Suresh',  lastName: 'Babu',      email: 'suresh.babu@xyz.com',     phone: '9811223344', department: 'Finance',     designation: 'Tax Consultant',       salary: 680000,  joinDate: '2019-03-25', status: 'Inactive' },
  { id: 13, firstName: 'Meera',   lastName: 'Krishnan',  email: 'meera.krishnan@xyz.com',  phone: '9822334455', department: 'Engineering', designation: 'QA Engineer',          salary: 710000,  joinDate: '2020-10-08', status: 'Active'   },
  { id: 14, firstName: 'Amit',    lastName: 'Joshi',     email: 'amit.joshi@xyz.com',      phone: '9833445566', department: 'Operations',  designation: 'Supply Chain Analyst', salary: 630000,  joinDate: '2023-07-14', status: 'Active'   },
  { id: 15, firstName: 'Pooja',   lastName: 'Ghosh',     email: 'pooja.ghosh@xyz.com',     phone: '9844556677', department: 'Operations',  designation: 'Process Engineer',     salary: 870000,  joinDate: '2024-01-20', status: 'Inactive' }
];

const initialAdminCredentials = {
  username: 'admin',
  password: 'admin123'
};
