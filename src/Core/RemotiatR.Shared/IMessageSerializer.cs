using System.IO;
using System.Threading.Tasks;

namespace RemotiatR.Shared
{
    public interface IMessageSerializer
    {
        Task<object> Deserialize(Stream stream);

        Task<Stream> Serialize(object value);
    }
}
