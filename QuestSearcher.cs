using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using reWZ;
using reWZ.WZProperties;
using System.Diagnostics;

namespace QuestViewer
{
    class QuestSearcher
    {
        public string wzFilePath { set; get; } = null;
        public string content { set; get; } = "";

        public string SearchByName(string name) // .wz menu
        {
            WZFile wz = new WZFile(wzFilePath + "/Quest.wz", WZVariant.Classic, true);
            WZObject wZObject = wz.ResolvePath("/QuestInfo.img");
            foreach(var x in wZObject)
            {
                try
                {
                    WZObject questName = x.ResolvePath("/name");
                    if (questName.ValueOrDie<string>().Contains(name))
                    {
                        content += questName.ValueOrDie<string>() + "\r\n\r\n";
                        WZObject say = wz.ResolvePath("/Say.img/"+x.Name);
                        foreach(var order in say)
                        {
                            foreach(var item in order)
                            {
                                if(item.Name.Contains("say"))
                                {
                                    foreach (var text in item)
                                    {
                                        content += text.ResolvePath("/msg").ValueOrDie<string>();
                                        try
                                        {
                                            int userSay = text.ResolvePath("/userSay").ValueOrDie<int>();
                                            if (userSay == 1)
                                                content += " (user say)";
                                        }
                                        catch (Exception e)
                                        {

                                        }
                                        finally
                                        {
                                            content += "\r\n\r\n";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch(Exception e)
                {

                }
            }
            return content;
        }

    }
}
