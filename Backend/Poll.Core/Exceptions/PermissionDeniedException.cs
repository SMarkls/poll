namespace Poll.Core.Exceptions;

public class PermissionDeniedException() : AppException("Доступ к этому ресурсу запрещен", 403);