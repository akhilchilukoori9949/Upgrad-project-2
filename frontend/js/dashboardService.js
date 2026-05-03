const dashboardService = (() => {

  async function getSummary() {
    return await storageService.getDashboard();
  }

  return { getSummary };

})();
