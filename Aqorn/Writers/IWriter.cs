using System.Text;

namespace Aqorn.Writers;

internal interface IWriter
{
    void Write(StringBuilder output);
}