using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frootify.PluginSystem
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class PluginAttribute : Attribute
    {
    }

}
