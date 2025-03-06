using System.Text;

namespace Aqorn.Models.Writers;

public interface IWriter
{
    void Write(StringBuilder output);
}