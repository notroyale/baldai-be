namespace baldai_be.Application.Features.Auth;

using baldai_be.Domain.Entities;

public interface IJwtProvider
{
    string Generate(User user);
}
