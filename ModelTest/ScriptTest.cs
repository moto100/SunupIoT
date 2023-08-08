using Sunup.ScriptExecutor;
using Sunup.Contract;
using System;
using Sunup.PlatformModel;
using System.Linq.Expressions;

namespace ModelTest
{
   public class ScriptTest 
    {

        public void Run()
        {
            string script = @"

            Root.业务模型.设备模型1.设备模型2.设备模型4.Value = 100;
 var aa = Root.业务模型.设备模型1.设备模型2.Value;

if (Root.业务模型.设备模型1.设备模型2.Value == 100)
            {
                Root.业务模型.设备模型1.设备模型2.Value = 1000;
 var bb = Root.业务模型.设备模型1.设备模型2.Value;
            }
else
{
Root.业务模型.设备模型1.设备模型2.Value = 1000;
 var cc = Root.业务模型.设备模型1.设备模型2.Value;
}
            function printTips()
            {
               // tips.forEach((tip, i) => console.log(`Tip ${ i}:` +tip));
            }

        function AAA()
        {
            Root.业务模型.设备模型1.设备模型2.Value = 1001;
        }
            ";




//            script = @"

//Root.业务模型.Childs[设备模型1'].Childs['设备模型2'].Value + 100

//            ";
            
            
            
            ScriptHelper scriptHelper = new ScriptHelper(script);
            scriptHelper.ExtractVariables();
        }
    }
}
