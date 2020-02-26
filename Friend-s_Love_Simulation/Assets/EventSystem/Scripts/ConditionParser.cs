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

        //var s = new Parser();
        //s.Start("4 == 1+(1+([3]-2)*5)/[2]");

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
    public enum Condition
    {
        Equal, NotEqual, Greater, Less, GreaterEqual, LessEqual
    }

    List<Node> side_formale = new List<Node>();
    public Condition state_cond;
    public bool errorCode = false;
    public bool isParsered = false;
    public bool isParsered_value = false;

    public int Eval_Value(int[] values)
    {
        if (!errorCode && !isParsered && isParsered_value)
        {
            return Analysis_formale(new Node(side_formale[0]), values);
        }

        return 0;
    }

    public bool Eval(int[] values)
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

    public void Start_Value(string formale)
    {
        Node node = new Node();
        isParsered_value = true;

        string fixed_formale = EraseSpace(formale);
        Node_Register(fixed_formale, node);
        side_formale.Add(node);
    }

    public void Start(string condition)
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

    /*
    private string[] Split_Condformale_Multiple()
    {
        string[] formales =
        {
            "==", "!=", "<=", ">=", "<", ">", "&&", "||"
        };
    }

    */

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

    private void Test(Node node, int i)
    {
        string sp = "";
        for (int j=0; j<i; j++)
        {
            sp += "   ";
        }

        string ns = "";
        string ss = "";
        string f = "";

        foreach(string num in node.numbers)
        {
            ns += (num+" ");
        }
        Debug.Log(sp+"numbers: " + ns);

        foreach (string sign in node.signs)
        {
            ss += sign;
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

        foreach (Node n in node.children)
        {
            Test(n, ++i);
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
