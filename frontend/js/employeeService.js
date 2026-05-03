const employeeService = (() => {

  async function getAll(query) {
    return await storageService.getAll(query || {});
  }

  async function getById(id) {
    return await storageService.getById(id);
  }

  async function add(data) {
    return await storageService.add(data);
  }

  async function update(id, data) {
    return await storageService.update(id, data);
  }

  async function remove(id) {
    return await storageService.remove(id);
  }

  return { getAll, getById, add, update, remove };

})();
