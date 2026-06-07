namespace Pindou.Application.Common;

/// <summary>
/// 业务异常
/// </summary>
public class BizException : Exception
{
    public int Code { get; }

    public BizException(string message, int code = 1000) : base(message)
    {
        Code = code;
    }

    public BizException(int code, string message) : base(message)
    {
        Code = code;
    }
}

/// <summary>
/// 通用错误码
/// </summary>
public static class ErrorCodes
{
    // 通用错误
    public const int Success = 0;
    public const int Unknown = -1;
    public const int ParamError = 1001;
    public const int MissingParam = 1002;

    // 认证错误 2001-2999
    public const int TokenInvalid = 2001;
    public const int TokenExpired = 2002;
    public const int NoPermission = 2003;
    public const int LoginFailed = 2010;
    public const int UserDisabled = 2011;
    public const int AccountLocked = 2012;
    public const int CaptchaError = 2013;
    public const int PhoneInvalid = 2014;

    // 资源错误 4001-4999
    public const int NotFound = 4001;
    public const int AlreadyExists = 4002;

    // 服务错误 5001-5999
    public const int ServerError = 5001;
    public const int DbError = 5002;
    public const int ExternalError = 5003;
}
