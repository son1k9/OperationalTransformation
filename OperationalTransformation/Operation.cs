using System.Diagnostics;
using System.Text;

namespace OperationalTransformation;

public enum OperationType
{
    None,
    Insert,
    Delete
}


//See https://en.wikipedia.org/wiki/Operational_transformation for explanation

public class Operation
{
    public readonly OperationType Type;
    public readonly int Pos;
    public readonly string Text;

    private Operation(OperationType type, int pos, string text)
    {
        Type = type;
        Pos = pos;
        Text = text;
    }

    public static Operation CreateNoneOp()
    {
        return new Operation(OperationType.None, -1, "");
    }

    public static Operation CreateInsertOp(int index, string text)
    {
        return new Operation(OperationType.Insert, index, text);
    }

    public static Operation CreateDeleteOp(int index, string text)
    {
        return new Operation(OperationType.Delete, index, text);
    }

    public static Operation TransformInsertInsert(Operation op1, Operation op2, bool op1Priority = false)
    {
        Debug.Assert(op1.Type == OperationType.Insert && op2.Type == OperationType.Insert);

        if (op1.Pos < op2.Pos || (op1.Pos == op2.Pos && op1Priority))
        {
            return op1;
        }

        return CreateInsertOp(op1.Pos + op2.Text.Length, op1.Text);
    }

    public static Operation TransformInsertDelete(Operation op1, Operation op2)
    {
        Debug.Assert(op1.Type == OperationType.Insert && op2.Type == OperationType.Delete);

        if (op1.Pos <= op2.Pos)
        {
            return op1;
        }

        if (op1.Pos > op2.Pos + op2.Text.Length - 1)
        {
            return CreateInsertOp(op1.Pos - op2.Text.Length, op1.Text);
        }

        return CreateNoneOp();
    }

    public static Operation TransformDeleteInsert(Operation op1, Operation op2)
    {
        Debug.Assert(op1.Type == OperationType.Delete && op2.Type == OperationType.Insert);

        if (op1.Pos + op1.Text.Length - 1 < op2.Pos)
        {
            return op1;
        }

        if (op1.Pos >= op2.Pos)
        {
            return CreateDeleteOp(op1.Pos + op2.Text.Length, op1.Text);
        }

        //Combine delete text and insert text 
        var prefix = op1.Text.AsSpan(0, op2.Pos - op1.Pos);
        var postfix = op1.Text.AsSpan(op2.Pos - op1.Pos, (op1.Pos + op1.Text.Length + op2.Text.Length - 1) - (op2.Pos + op2.Text.Length - 1));
        var str = new StringBuilder().Append(prefix).Append(op2.Text).Append(postfix).ToString();

        return CreateDeleteOp(op1.Pos, str);
    }

    public static Operation TransformDeleteDelete(Operation op1, Operation op2)
    {
        Debug.Assert(op1.Type == OperationType.Delete && op2.Type == OperationType.Delete);

        if (op1.Pos == op2.Pos && op1.Text.Length == op2.Text.Length)
        {
            return CreateNoneOp();
        }

        //If op1 is to the left or to the right of op2 just adjust op1.pos if needed
        if (op1.Pos + op1.Text.Length - 1 < op2.Pos)
        {
            return op1;
        }
        if (op1.Pos > op2.Pos + op2.Text.Length - 1)
        {
            return CreateDeleteOp(op1.Pos - op2.Text.Length, op1.Text);
        }

        //If op2 is before or at the same pos as op1 delete only what was not deleted by op2
        if (op2.Pos <= op1.Pos)
        {
            if (op1.Pos + op1.Text.Length <= op2.Pos + op2.Text.Length)
            {
                return CreateNoneOp();
            }

            return CreateDeleteOp(op2.Pos, op1.Text.Substring(op2.Pos + op2.Text.Length - op1.Pos));
        }


        //If op2 is after op1 delete only what was not deleted by op2
        if (op2.Pos + op2.Text.Length < op1.Pos + op1.Text.Length)
        {
            var prefix = op1.Text.AsSpan(0, op2.Pos - op1.Pos);
            var postfix = op1.Text.AsSpan(op2.Pos - op1.Pos + op2.Text.Length, op1.Text.Length - prefix.Length - op2.Text.Length);
            var str = new StringBuilder().Append(prefix).Append(postfix).ToString();
            return CreateDeleteOp(op1.Pos, str);
        }

        return CreateDeleteOp(op1.Pos, op1.Text.Substring(0, op2.Pos - op1.Pos));
    }
}

