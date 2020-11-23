using System;
using System.Collections.Specialized;

namespace FlexKidsConnection
{
    public interface IWeb : IDisposable
    {
        byte[] PostValues(string address, NameValueCollection data);
        string DownloadPageAsString(string address);
    }
}