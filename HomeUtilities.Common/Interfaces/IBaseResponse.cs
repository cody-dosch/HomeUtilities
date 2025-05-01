using System;
using System.Net;
using Newtonsoft.Json;

namespace HomeUtilities.Common.Interfaces
{
    public interface IBaseResponse
    {
        bool Success { get; set; }

        Exception DALException { get; set; }
    }
}
