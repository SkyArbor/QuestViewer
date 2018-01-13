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
        public string questIds { set; get; } = "";

        private string AnalyseSay(WZObject node)
        {
            if (node.ChildCount == 0)
                return "";
            string result = "["+node.Name+"] begin\r\n\r\n";
            foreach(var item in node)
            {
                result += item.ValueOrDefault<string>("") + "\r\n\r\n";
            }
            result += "[" + node.Name + "] end\r\n\r\n";
            return result;
        }

        private string AnalyseSayEx(WZObject node)
        {
            if (node.ChildCount == 0)
                return "";
            string result = "[" + node.Name + "] begin\r\n\r\n";
            foreach (var property in node)
            {
                result += property.ResolvePath("/msg").ValueOrDefault<string>("Error in AnalyseSayEx.");
                try
                {
                    if (property.ResolvePath("/userSay").ValueOrDie<int>() == 1)
                        result += " (user say)\r\n\r\n";
                }
                catch (Exception e)
                {

                }
                finally
                {
                    result += "\r\n\r\n";
                }
            }
            result += "[" + node.Name + "] end\r\n\r\n";
            return result;
        }
        private string AnalyseSay_All(WZObject questId)
        {
            string result = "";
            foreach(var order in questId)
            {
                foreach (var item in order)
                {
                    if (item.Type == WZObjectType.String)
                    {
                        result += item.ValueOrDie<string>() +"\r\n\r\n";
                    }
                    else if(item.Type==WZObjectType.SubProperty)
                    {
                        if (item.Name[item.Name.Length - 1] == 'x' && item.Name[item.Name.Length - 2] == 'E')
                            result += AnalyseSayEx(item);
                        else
                            result += AnalyseSay(item);
                    }
                }
            }
            return result;
        }

        public string SearchByName(string name) // .wz menu
        {
            WZFile wz = new WZFile(wzFilePath + "/Quest.wz", WZVariant.Classic, true);
            WZObject wZObject = wz.ResolvePath("/QuestInfo.img");
            content = "";
            questIds = "";
            foreach (var questID in wZObject)
            {
                try
                {
                    WZObject questName = questID.ResolvePath("/name");
                    if (questName.ValueOrDie<string>().Contains(name))
                    {
                        content += questName.ValueOrDie<string>() + "\r\n\r\n";
                        content += AnalyseSay_All(wz.ResolvePath("/Say.img/" + questID.Name));
                        questIds += questID.Name;
                    }
                }
                catch (Exception e)
                {

                }
            }
            return content;
        }
    }
}
