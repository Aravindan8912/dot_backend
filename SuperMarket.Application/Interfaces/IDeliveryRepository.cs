public interface IDeliveryRepository{
    Task AddAsync(Delivery delivery);
    Task<Delivery> GetByOrderIdAsync(Guid orderId);
    Task UpdateAsync(Delivery delivery);
    Task DeleteAsync(Delivery delivery);
}