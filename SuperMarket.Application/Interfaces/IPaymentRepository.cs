public interface IPaymentRepository{
    Task AddAsync(Payment payment);
    Task<Payment> GetByOrderIdAsync(Guid orderId);
    Task UpdateAsync(Payment payment);
    Task DeleteAsync(Payment payment);
}