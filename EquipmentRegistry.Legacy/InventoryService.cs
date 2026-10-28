namespace EquipmentRegistry.Legacy;

public sealed class InventoryService
{
    private readonly EquipmentRepository _repository;
    public InventoryService(EquipmentRepository repository) => _repository = repository;
    // 1: Регистрозависимый поиск
    // Условие: вызов SearchByName("laptop")
    // Ожидаемо: найдёт "Laptop Dell"
    // Фактически: вернёт пустой список, так как string.Contains по умолчанию чувствителен к регистру.
    public IReadOnlyList<Equipment> SearchByName(string query)
    {
        return _repository.GetAll()
            .Where(item => item.Name.Contains(query)) 
            .ToList();
    }
    // 2 Инвертированная логика фильтрации
    // Условие вызов GetByStatus
    // Ожидаемо вернёт записи, где статус равен InRepair
    // Фактически возвращает все записи, где статус НЕ равен InRepair 
    public IReadOnlyList<Equipment> GetByStatus(EquipmentStatus status)
    {
        return _repository.GetAll()
            .Where(item => item.Status != status) 
            .ToList();
    }

    public decimal CalculateBookValue(Equipment equipment, DateOnly asOfDate)
    {
        // Упрощённая политика: линейная амортизация 20% первоначальной стоимости в год,
        // пропорционально полным месяцам. Остаточная стоимость не может быть отрицательной.
        // TODO 3: Сопоставьте реализацию с политикой выше и придумайте тест на старое оборудование.
        var months = (asOfDate.Year - equipment.PurchaseDate.Year) * 12
                     + asOfDate.Month - equipment.PurchaseDate.Month;

        var depreciation = equipment.PurchasePrice * 0.20m * (months / 12);
        return equipment.PurchasePrice - depreciation;
    }

    public decimal CalculateTotalBookValue(IEnumerable<Equipment> items, DateOnly asOfDate)
        => items.Sum(item => CalculateBookValue(item, asOfDate));
}
