using System;
using Xioa.Admin.Request.Tools.NetAxios;

namespace Xioa.Admin.Core.Services.Tokens;

///<summary>
/// @author：XIOA (xioa.liu)
/// @date：2024-12-28
/// @belong-sln：Xioa.Admin.System.Core 
/// @desc：ApplicationAxios
///</summary>
public static class ApplicationAxios
{
    private static IAxios? _axios;

    public static IAxios Axios
    {
        get
            => _axios ??= new NAxios(AxiosConfig, IgnoreSslErrorsSslError);
    }

    private static NAxiosConfig? _axiosConfig;

    private static NAxiosConfig? AxiosConfig
    {
        get
        {
            if (_axiosConfig is null)
                throw new NullReferenceException();
            return _axiosConfig;
        }
    }

    private static bool IgnoreSslErrorsSslError { get; set; }

    public static void SetAxiosConfig(NAxiosConfig? nAxiosConfig, bool sslError = false)
    {
        IgnoreSslErrorsSslError = sslError;
        _axiosConfig = nAxiosConfig;
    }
}