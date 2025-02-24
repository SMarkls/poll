using Microsoft.AspNetCore.Http;

namespace Poll.Core.Exceptions;

public class NotFoundException(string id, Type type)
    : AppException($"Сущность {type.Name} с идентификатором {id} не найдена", StatusCodes.Status404NotFound);