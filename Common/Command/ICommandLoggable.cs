using Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Command
{
    public interface ICommandLoggable
    {
        CommandLog ToCommandLog();
    }

}
