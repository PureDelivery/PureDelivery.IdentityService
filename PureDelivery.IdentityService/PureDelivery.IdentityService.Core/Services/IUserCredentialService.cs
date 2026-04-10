using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.DTOs.Identity.Requests;
using PureDelivery.Shared.Contracts.DTOs.Identity.Responses;

namespace PureDelivery.IdentityService.Core.Services
{
    /// <summary>
    /// Управление учётными данными пользователей (Manager, Courier).
    /// Вызывается внутренними сервисами при создании нового сотрудника.
    /// </summary>
    public interface IUserCredentialService
    {
        /// <summary>
        /// Создаёт учётные данные для менеджера или курьера.
        /// Аккаунт сразу активен (IsActive = true, IsEmailConfirmed = true),
        /// т.к. создаётся администратором / другим сервисом.
        /// </summary>
        Task<BaseResponse<RegisterUserCredentialResult>> RegisterAsync(
            RegisterUserCredentialRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Удаляет (деактивирует) учётные данные пользователя по UserId.
        /// </summary>
        Task<BaseResponse<bool>> DeactivateAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
