using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class ConditionParser : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int[] vs =
        {
            10, 9, 8, 7, 6, 5, 4, 3, 2, 1
        };

        string puttern = "(0 != 5 || (4 + [4] != [0] || 0==0)) && 4 < 6 && 7==7";
        var s = new Parser();
        //s.Start(puttern);
        s.Start_Multi(puttern);
        Debug.Log(s.Eval_Multi(vs));


        //Debug.Log("ERROR: " + s.errorCode);
        //Debug.Log(s.Eval(vs));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public sealed class Parser
{
    public Parser()
    {

    }

    public Parser(string formale)
    {
        Start(formale);
    }

    public enum Condition
    {
        Equal, NotEqual, Greater, Less, GreaterEqual, LessEqual
    }

    List<Node> side_formale = new List<Node>();
    Node root;
    public Condition state_cond;
    public bool errorCode = false;
    public bool isParsered = false;
    public bool isParsered_value = false;

    public bool Eval(int[] values)
    {
        return Eval_Multi(values);
    }

    public int Eval_Value(int[] values)
    {
        if (!errorCode && !isParsered && isParsered_value)
        {
            return Analysis_formale(new Node(side_formale[0]), values);
        }

        return 0;
    }

    public bool Eval_old(int[] values)
    {
        int left = Analysis_formale(new Node(side_formale[0]), values);
        int right = Analysis_formale(new Node(side_formale[1]), values);

        //Debug.LogFormat("{0} {1} {2}", left, state_cond, right);
        //Debug.Log("isParsered: "+isParsered);

        if (!errorCode && isParsered && !isParsered_value)
        {
            switch (state_cond)
            {
                case Condition.Equal:
                    return left == right;

                case Condition.NotEqual:
                    return left != right;

                case Condition.Greater:
                    return left < right;

                case Condition.Less:
                    return left > right;

                case Condition.GreaterEqual:
                    return left <= right;

                case Condition.LessEqual:
                    return left >= right;
            }
        }

        return false;
    }
    
    public bool Eval_Multi(int[] values)
    {
        return Analysis_formale_Multi(root, values);
    }

    private bool Analysis_formale_Multi(Node baseNode, int[] values)
    {
        bool left;
        bool right;
        //int index_child = 0;
        bool outBool = false;

        if (baseNode.signs.Count != 0)
        {
            //Test_Single(baseNode);
            for (int i = 0; i < baseNode.signs.Count; i++)
            {
                /*
                Debug.Log("第一段階元ノード");
                Test_Single(baseNode);
                Debug.Log("第一段階子供1ノード");
                Test_Single(baseNode.children[i]);
                Debug.Log("第一段階子供2ノード");
                Test_Single(baseNode.children[i + 1]);
                */

                switch (baseNode.signs[i])
                {
                    case "&&":
                        left = Side_Nomber_Multi(baseNode.children[i], i, values);
                        right = Side_Nomber_Multi(baseNode.children[i+1], i + 1, values);
                        outBool = left && right;

                        break;
                    case "||":
                        left = Side_Nomber_Multi(baseNode.children[i], i, values);
                        right = Side_Nomber_Multi(baseNode.children[i+1], i + 1, values);
                        outBool = left || right;

                        break;
                }
            }
        }
        else
        {
            int d = 0;
            outBool = Side_Nomber_Multi(baseNode.children[0], 0, values);
        }

        //Debug.Log(outBool);
        return outBool;
    }
    

    private int Analysis_formale(Node node, int[] values)
    {
        int left = 0;
        int right = 0;
        int index_child = 0;
        int outValue = 0;

        //Test(node, 1);

        for (int i=0; i < node.signs.Count; i++)
        {
            //Debug.Log("現在の符号: "+node.signs[i]);
            if (node.signs[i] == "*" || node.signs[i] == "/" || node.signs[i] == "%")
            {
                //Debug.Log("両辺代入");
                left = Side_Nomber(node, i, ref index_child, values);
                right = Side_Nomber(node, i + 1, ref index_child, values);
            }

            switch (node.signs[i])
            {
                case "*":
                    node.numbers[i] = (left * right).ToString();
                    //Debug.LogFormat("{0} * {1} = {2}", left, right, node.numbers[i]);
                    node.numbers.RemoveAt(i + 1);
                    node.index_flags[i] = false;
                    node.index_flags.RemoveAt(i + 1);
                    node.signs.RemoveAt(i);
                    break;
                case "/":
                    node.numbers[i] = (left / right).ToString();
                    //Debug.LogFormat("{0} / {1} = {2}", left, right, node.numbers[i]);
                    node.numbers.RemoveAt(i + 1);
                    node.index_flags[i] = false;
                    node.index_flags.RemoveAt(i + 1);
                    node.signs.RemoveAt(i);
                    break;
                case "%":
                    node.numbers[i] = (left % right).ToString();
                    //Debug.LogFormat("{0} % {1} = {2}", left, right, node.numbers[i]);
                    node.numbers.RemoveAt(i + 1);
                    node.index_flags[i] = false;
                    node.index_flags.RemoveAt(i + 1);
                    node.signs.RemoveAt(i);
                    break;
            }
        }

        //Test(node, 1);

        for (int i = 0; i < node.signs.Count; i++)
        {
            //Debug.Log("現在の符号: " + node.signs[i]);

            if (node.signs[i] == "+" || node.signs[i] == "-")
            {
                left = Side_Nomber(node, i, ref index_child, values);
                right = Side_Nomber(node, i + 1, ref index_child, values);
            }

            switch (node.signs[i])
            {
                case "+":
                    node.numbers[i] = (left + right).ToString();
                    //Debug.LogFormat("{0} + {1} = {2}", left, right, node.numbers[i]);
                    node.numbers.RemoveAt(i + 1);
                    node.index_flags[i] = false;
                    node.index_flags.RemoveAt(i + 1);
                    node.signs.RemoveAt(i);
                    break;
                case "-":
                    node.numbers[i] = (left - right).ToString();
                    //Debug.LogFormat("{0} - {1} = {2}", left, right, node.numbers[i]);
                    node.numbers.RemoveAt(i + 1);
                    node.index_flags[i] = false;
                    node.index_flags.RemoveAt(i + 1);
                    node.signs.RemoveAt(i);
                    break;
            }
        }
        //Test(node, 1);
        //Debug.Log("flag_count: "+ node.index_flags.Count);

        if (node.index_flags[0])
        {
            outValue = values[int.Parse(node.numbers[0])];
        }
        else
        {
            outValue = int.Parse(node.numbers[0]);
        }
        //Debug.Log("答え: " + outValue);
        return outValue;
    }

    private bool Side_Nomber_Multi(Node node, int index, int[] values)
    {
        bool outBool = false;

        if (index < node.parent.numbers.Count)
        {
            if (node.parent.numbers[index] == "#")
            {
                

                outBool = Analysis_formale_Multi(node, values);
            }
            else
            {
                /*
                Debug.Log("第二段階元ノード");
                Test_Single(node);
                Debug.Log("第二段階子供1ノード");
                Test_Single(node.children[0]);
                Debug.Log("第二段階子供2ノード");
                Test_Single(node.children[1]);
                */

                string sign = node.signs[0];

                int left = Analysis_formale(new Node(node.children[0]), values);
                int right = Analysis_formale(new Node(node.children[1]), values);

                switch (sign)
                {
                    case "==":
                        outBool = left == right;
                        break;
                    case "!=":
                        outBool = left != right;
                        break;
                    case "<=":
                        outBool = left <= right;
                        break;
                    case ">=":
                        outBool = left >= right;
                        break;
                    case "<":
                        outBool = left < right;
                        break;
                    case ">":
                        outBool = left > right;
                        break;
                }
            }
        }
        //Debug.Log(outBool);
        return outBool;
    }



    private int Side_Nomber(Node node, int index, ref int index_child, int[] values)
    {
        int outNumber = 0;

        if (index < node.numbers.Count)
        {
            if (node.numbers[index] == "#")
            {
                outNumber = Analysis_formale(node.children[index_child++], values);
            }
            else
            {
                try
                {
                    if (node.index_flags[index])
                    {
                        if (int.Parse(node.numbers[index]) < values.Length)
                        {
                            outNumber = values[int.Parse(node.numbers[index])];
                        }
                        else
                        {
                            errorCode = true;
                        }
                    }
                    else
                    {
                        outNumber = int.Parse(node.numbers[index]);
                    }
                }
                catch
                {
                    errorCode = true;
                }
            }
        }

        return outNumber;
    }

    public void Start(string formale)
    {
        Start_Multi(formale);
    }

    public void Start_Value(string formale)
    {
        Node node = new Node();
        isParsered_value = true;

        string fixed_formale = EraseSpace(formale);
        Node_Register(fixed_formale, node);
        side_formale.Add(node);
    }

    /// <summary>
    /// 単体条件式限定
    /// </summary>
    /// <param name="condition"></param>
    public void Start_old(string condition)
    {
        if (isParsered_value) return;

        Node left_node = new Node(); //最初のノード
        Node right_node = new Node(); //最初のノード
        isParsered = true;

        string fixed_formale = EraseSpace(condition);
        //Debug.Log("fixed formale: " + fixed_formale);

        List<string> side = new List<string>(Split_Condformale(fixed_formale));


        if (side.Count == 3)
        {
            side_formale.Add(left_node);
            Node_Register(side[0], left_node);

            side_formale.Add(right_node);
            Node_Register(side[2], right_node);

            switch (side[1])
            {
                case "==":
                    state_cond = Condition.Equal;
                    break;
                case "!=":
                    state_cond = Condition.NotEqual;
                    break;
                case "<=":
                    state_cond = Condition.GreaterEqual;
                    break;
                case ">=":
                    state_cond = Condition.LessEqual;
                    break;
                case "<":
                    state_cond = Condition.Greater;
                    break;
                case ">":
                    state_cond = Condition.Less;
                    break;
            }
        }

        //Test(left_node, 0);
        //Test(right_node, 0);
        side_formale.Add(left_node);
        side_formale.Add(right_node);
    }

    /// <summary>
    /// 複数条件式対応
    /// </summary>
    /// <param name="condition"></param>
    public void Start_Multi(string condition)
    {
        Node node = new Node();
        root = node;
        Node_Register_Multi(EraseSpace(condition), node);

        //Test(node, 0);
    }

    private string EraseSpace(string test)
    {
        string outText = "";

        foreach(char c in test)
        {
            if (!Regex.IsMatch(c.ToString(), @"\s"))
            {
                outText += c.ToString();
            }
        }

        return outText;
    }

    /// <summary>
    /// 条件式で二つに分ける
    /// </summary>
    /// <returns></returns>
    private string[] Split_Condformale(string condition)
    {
        string[] vs;
        List<string> outList = new List<string>();

        string[] formales =
        {
            "==", "!=", "<=", ">=", "<", ">"
        };


        foreach (string s in formales) {
            if (condition.Contains(s))
            {
                outList = new List<string>(Regex.Split(condition, s));
                outList.Insert(1, s);
                break;
            }
        }

        return outList.ToArray();
    }

    private void Node_Register_Formule(string condition, Node baseNode)
    {
        Node target_node = baseNode;

        string buff = "";
        for (int i = 0; i < condition.Length; i++)
        {
            buff += condition[i]; //一文字ずつ追加

            //値一致
            if (Regex.IsMatch(buff, @"(\[\d{1,4}\]|\d{1,4})"))
            {
                target_node.numbers.Add(buff);
                buff = "";
            }

            //andor演算子
            if (Regex.IsMatch(buff, @"&&|\|\||and|or"))
            {
                target_node.signs.Add(buff);
                buff = "";
            }

            //
            if (condition[i] == '(')
            {
                target_node.numbers.Add("#");
                var node = new Node();
                target_node.Add(node);
                buff = "";

                target_node = node;

            }

            if (condition[i] == ')')
            {
                buff = "";
                target_node = target_node.parent;
            }
        }

        //最終追加
        if (buff != "")
        {
            target_node.numbers.Add(buff);
            buff = "";
        }
    }

    private void Node_Register_Multi(string condition, Node baseNode)
    {
        Node target_node = baseNode;

        string buff = "";
        for (int i = 0; i < condition.Length; i++)
        {
            buff += condition[i]; //一文字ずつ追加

            /*
            //値一致
            if (Regex.IsMatch(buff, @".+(==|!=|<=|>=|<|>).+"))
            {
                var rootNode = new Node();
                var leftNode = new Node();
                var tightNode = new Node();

                Split_Condformale(buff)

                //rootNode.Add()


                Node_Register(buff, rootNode);
                target_node.Add(rootNode);
                target_node.numbers.Add(buff);
                buff = "";
            }
            */

            //andor演算子
            if (Regex.IsMatch(buff, @"&&|\|\|"))
            {

                string s = Eliminate_String(buff, 2);
                //target_node.numbers.Add(s);
                Add_Formule(target_node, s);
                target_node.signs.Add(buff[buff.Length-1].ToString()+buff[buff.Length-2].ToString());
                buff = "";
            }

            //
            if (condition[i] == '(')
            {
                target_node.numbers.Add("#");
                var node = new Node();
                target_node.Add(node);
                buff = "";

                target_node = node;

            }

            if (condition[i] == ')')
            {
                string s = Eliminate_String(buff, 1);
                //target_node.numbers.Add(s);
                Add_Formule(target_node, s);
                buff = "";
                target_node = target_node.parent;
            }
        }

        //最終追加
        if (buff != "")
        {
            Add_Formule(target_node, buff);
            // target_node.numbers.Add(buff);
            buff = "";
        }

        //Test(target_node, 0);
    }

    private void Add_Formule(Node node, string formule)
    {
        if (formule == "") return;

        //Debug.LogFormat("[{0}]", formule);

        node.numbers.Add(formule);

        var r_node = new Node();
        node.Add(r_node);

        List<string> side = new List<string>(Split_Condformale(formule));

        /*
        Debug.Log("Start");
        foreach(string s in side)
        {
            Debug.Log("side : "+s);
        }
        Debug.Log("End");
        */

        r_node.numbers.Add(side[0]);
        r_node.signs.Add(side[1]);
        r_node.numbers.Add(side[2]);

        var left = new Node();
        var right = new Node();

        Node_Register(side[0], left);
        Node_Register(side[2], right);

        r_node.Add(left);
        r_node.Add(right);
    }

    private string Eliminate_String(string formule, int time)
    {
        string outString = "";
        int roopTime = 0;

        foreach(char c in formule)
        {
            if (roopTime > formule.Length - time-1)
            {
                break;
            }

            outString += c;

            roopTime++;
        }

        return outString;
    }

    private string[] Split_Condformale_Multiple(string condition)
    {
        string[] formales =
        {
            "==", "!=", "<=", ">=", "<", ">", "&&", "||"
        };

        var regex = new Regex(@"(\[\d{1,4}\]|\d{1,4})(==|!=|<=|>=|<|>)(\[\d{1,4}\]|\d{1,4})", RegexOptions.IgnorePatternWhitespace);
        var rc = regex.Matches(condition);

        List<string> number = new List<string>();
        

        return null;
    }

    private void Node_Register(string farmale, Node baseNode)
    {
        string buff = "";
        Node target = baseNode;

        foreach(char c in farmale)
        {
            switch (c)
            {
                case '+':
                case '-':
                case '*':
                case '/':
                case '%':
                    Add_Target(buff, target);
                    target.signs.Add(c.ToString());
                    buff = "";

                    break;
                case '(':
                    {
                        target.numbers.Add("#");
                        target.index_flags.Add(false);

                        Node node = new Node();
                        target.Add(node);
                        target = node;
                    }
                    break;
                case ')':
                    Add_Target(buff, target);
                    buff = "";
                    target = target.parent;
                    break;

                default:
                    buff += c;

                    break;
            }
        }

        if (buff != "") Add_Target(buff, target);

    }

    private void Add_Target(string buff, Node target)
    {
        if (Regex.IsMatch(buff, @"\d{1,4}"))
        {
            string s = "";
            foreach(char c in buff)
            {
                if (c!='[' && c != ']')
                {
                    s += c.ToString();
                }
            }

            target.numbers.Add(s);

            if (Regex.IsMatch(buff, @"\[\d{1,4}\]"))
            {
                target.index_flags.Add(true);
            }
            else
            {
                target.index_flags.Add(false);
            }
        }

    }

    private void Test_Single(Node node)
    {
        Debug.Log("+=<START>==================================+");
        string ns = "";
        string ss = "";
        string f = "";

        foreach (string num in node.numbers)
        {
            ns += (num + ", ");
        }
        Debug.Log("numbers: " + ns);

        foreach (string sign in node.signs)
        {
            ss += sign + ", ";
        }
        Debug.Log("signs: " + ss);

        foreach (bool flag in node.index_flags)
        {
            if (flag)
            {
                f += "T";
            }
            else
            {
                f += "F";
            }
        }
        Debug.Log("flag: " + f);

        Debug.Log("+=<END>===================================+");
    }

    private void Test(Node node, int i)
    {
        string sp = "";
        for (int j=0; j<i; j++)
        {
            sp += "〇";
        }

        string ns = "";
        string ss = "";
        string f = "";

        foreach(string num in node.numbers)
        {
            ns += (num+", ");
        }
        Debug.Log(sp+"numbers: " + ns);

        foreach (string sign in node.signs)
        {
            ss += sign+", ";
        }
        Debug.Log(sp+"signs: " + ss);

        foreach (bool flag in node.index_flags)
        {
            if (flag)
            {
                f += "T";
            }
            else
            {
                f += "F";
            }
        }
        Debug.Log(sp + "flag: " + f);

        i++;
        foreach (Node n in node.children)
        {
            Test(n, i);
        }
    }

    public class Node
    {
        public List<string> numbers = new List<string>();
        public List<string> signs = new List<string>();
        public List<bool> index_flags = new List<bool>();
        public Node parent;
        public List<Node> children = new List<Node>();

        public Node()
        {
            numbers = new List<string>();
            signs = new List<string>();
            index_flags = new List<bool>();
            children = new List<Node>();
            parent = null;
        }

        public Node(Node node)
        {
            numbers = new List<string>(node.numbers);
            signs = new List<string>(node.signs);
            index_flags = new List<bool>(node.index_flags);
            children = new List<Node>();

            foreach (Node n in node.children)
            {
                var n1 = new Node(n)
                {
                    parent = n
                };
                children.Add(n1);
            }
        }

        public void Add(Node node)
        {
            children.Add(node);
            node.parent = this;
        }
    }
}
