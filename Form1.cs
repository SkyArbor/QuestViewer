using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using reWZ;
using reWZ.WZProperties;
using QuestViewer;
using System.IO;

namespace QuestViewer
{
    
    public partial class Form1 : Form
    {
        QuestSearcher questSearcher = new QuestSearcher();
        public SearchArgs searchArgs = new SearchArgs();
        public Form1()
        {
            InitializeComponent();
        }

 /*       private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog o = new OpenFileDialog())
            {
                o.Filter = "wz File|*.wz";
                if(o.ShowDialog()==DialogResult.OK)
                {
                    readwz(o.FileName);
                    questSearcher.wzFilePath = Path.GetFullPath(o.FileName+"Quest.wz");
                    this.Text = o.FileName;
                }
            }
        }
        */
        private void readwz(string file)
        {
            try
            {
                WZFile wz = new WZFile(file, WZVariant.Classic, true);
                treeView1.Nodes.Clear();
                TreeNode node = new TreeNode(file.Split('\\')[file.Split('\\').Length - 1]);
                foreach (var x in wz.MainDirectory)
                {                  
                    switch (x.Type)
                    {
                        case WZObjectType.Image:
                            node.Nodes.Add(x.Name);
                            break;
                        case WZObjectType.Directory:
                            readwzDirectory(node, x);
                            break;
                        default:
                            node.Nodes.Add(x.Name);
                            break;
                    }                  
                }
                treeView1.Nodes.Add(node);                
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void readwzDirectory(TreeNode node,WZObject wZObject)
        {
            foreach(var x in wZObject)
            {
                TreeNode subnode = new TreeNode(x.Name);
                switch (x.Type)
                {
                    case WZObjectType.Image:
                        break;
                    case WZObjectType.Directory:
                        readwzDirectory(node, x);
                        break;
                    default:
                        break;
                }
                node.Nodes.Add(subnode);
            }
        }

        private void readwzImg(WZObject wZObject)
        {
            treeView2.Nodes.Clear();
            TreeNode node = new TreeNode(wZObject.Name);
            foreach (var x in wZObject)
            {
                if (x.Type == WZObjectType.SubProperty)
                    readwzImgDirectory(node, x);
                else
                    node.Nodes.Add(x.Name);            
            }
            treeView2.Nodes.Add(node);
        }

        private void readwzImgDirectory(TreeNode node,WZObject wZObject)
        {
            TreeNode addNode = new TreeNode(wZObject.Name);
            foreach(var x in wZObject)
            {
                if (x.Type == WZObjectType.SubProperty)
                    readwzImgDirectory(addNode, x);
                else
                    addNode.Nodes.Add(x.Name);
            }
            node.Nodes.Add(addNode);
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            WZFile wz = new WZFile(this.Text, WZVariant.Classic, true);
            string path = getNodepath(e.Node, "");
            if (path == "")
                return;
            WZObject wZObject = wz.ResolvePath(path.Replace('\\', '/'));
            if (wZObject.Type != WZObjectType.Directory)
                readwzImg(wZObject);
        }

        private void OpenwzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog o = new OpenFileDialog())
            {
                o.Filter = "wz File|*.wz";
                if (o.ShowDialog() == DialogResult.OK)
                {
                    readwz(o.FileName);
                    questSearcher.wzFilePath = Path.GetDirectoryName(o.FileName);
                    this.Text = o.FileName;
                }
            }
        }

        private void treeView2_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Nodes.Count == 0)
            {
                WZFile wz = new WZFile(questSearcher.wzFilePath, WZVariant.Classic, true);               
                WZObject wZObject = wz.ResolvePath(e.Node.Parent.FullPath.Replace('\\', '/')+'/'+e.Node.Text);
                MessageBox.Show(wZObject.ValueOrDefault<string>("string"));
            }
        }

        private string getNodepath(TreeNode node,string path,bool first=false)
        {
            if (node.Parent == null)
                return path;
            else
            {
                if (first == true)
                    path = node.Text + '/' + path;
                else
                    path = node.Text + path;
                getNodepath(node.Parent, path,true);
                return path;
            }
        }

        public class SearchArgs
        {
            public int type { set; get; }
            public string level { get; set; }
            public string name { get; set; }
        }

        private void ChangeSearchArgs(string name, string level, int type)
        {
            this.searchArgs.name = name;
            this.searchArgs.level = level;
            this.searchArgs.type = type;
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.getValue = ChangeSearchArgs;
            form2.ShowDialog();
            string test = questSearcher.SearchByName(searchArgs.name);
            File.WriteAllText(questSearcher.wzFilePath + "/" + searchArgs.name + ".txt", test);
            MessageBox.Show("Finished!\r\nPath: "+ questSearcher.wzFilePath + "/" + searchArgs.name + ".txt");
        }
    }
}
