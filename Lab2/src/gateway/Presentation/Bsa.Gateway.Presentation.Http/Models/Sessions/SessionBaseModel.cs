using System.Text.Json.Serialization;

namespace Bsa.Gateway.Presentation.Http.Models.Sessions;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(UserSessionModel), "user")]
[JsonDerivedType(typeof(AdminSessionModel), "admin")]
public abstract record SessionBaseModel(Guid Id, DateTimeOffset CreatedAt);