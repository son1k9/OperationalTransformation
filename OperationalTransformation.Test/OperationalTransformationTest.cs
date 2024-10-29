namespace OperationalTransformation.Test;

public class OperationalTransformationTest
{
    private static string ApplyOp(string str, Operation op)
    {
        if (op.Type == OperationType.Insert)
        {
            return str.Insert(op.Pos, op.Text);
        }

        if (op.Type == OperationType.Delete)
        {
            return str.Remove(op.Pos, op.Text.Length);
        }

        return str;
    }

    [Fact]
    public void InsertionInsertion_Test()
    {
        //Test same position
        {
            string str = "Some text";
            string str2 = str;
            string result = "STextTxetome text";

            var op1 = Operation.CreateInsertOp(1, "Text");
            var op2 = Operation.CreateInsertOp(1, "Txet");

            var op1Transformed = Operation.TransformInsertInsert(op1, op2, true);
            var op2Transformed = Operation.TransformInsertInsert(op2, op1);

            str = ApplyOp(ApplyOp(str, op1), op2Transformed);
            str2 = ApplyOp(ApplyOp(str2, op2), op1Transformed);

            Assert.Equal(str, str2);
            Assert.Equal(result, str);
        }

        //Test op1 position before op2 position
        {
            string str = "Some text";
            string str2 = str;
            string result = "STextoTxetme text";

            var op1 = Operation.CreateInsertOp(1, "Text");
            var op2 = Operation.CreateInsertOp(2, "Txet");

            var op1Transformed = Operation.TransformInsertInsert(op1, op2, true);
            var op2Transformed = Operation.TransformInsertInsert(op2, op1);

            str = ApplyOp(ApplyOp(str, op1), op2Transformed);
            str2 = ApplyOp(ApplyOp(str2, op2), op1Transformed);

            Assert.Equal(str, str2);
            Assert.Equal(result, str);
        }

        //Test op1 position after op2 position
        {
            string str = "Some text";
            string str2 = str;
            string result = "STxetoTextme text";

            var op1 = Operation.CreateInsertOp(2, "Text");
            var op2 = Operation.CreateInsertOp(1, "Txet");

            var op1Transformed = Operation.TransformInsertInsert(op1, op2, true);
            var op2Transformed = Operation.TransformInsertInsert(op2, op1);

            str = ApplyOp(ApplyOp(str, op1), op2Transformed);
            str2 = ApplyOp(ApplyOp(str2, op2), op1Transformed);

            Assert.Equal(str, str2);
            Assert.Equal(result, str);
        }
    }

    [Fact]
    public void InsertionDeletionAndDeletionInsert_Test()
    {
        //Test insertion before deletion
        {
            string str = "Some text";
            string str2 = str;
            string result = "SText text";

            var op1 = Operation.CreateInsertOp(1, "Text");
            var op2 = Operation.CreateDeleteOp(1, "ome");

            var op1Transformed = Operation.TransformInsertDelete(op1, op2);
            var op2Transformed = Operation.TransformDeleteInsert(op2, op1);

            str = ApplyOp(ApplyOp(str, op1), op2Transformed);
            str2 = ApplyOp(ApplyOp(str2, op2), op1Transformed);

            Assert.Equal(str, str2);
            Assert.Equal(result, str);
        }

        //Test insertion after deletion
        {
            string str = "Some text";
            string str2 = str;
            string result = "SText text";

            var op1 = Operation.CreateInsertOp(4, "Text");
            var op2 = Operation.CreateDeleteOp(1, "ome");

            var op1Transformed = Operation.TransformInsertDelete(op1, op2);
            var op2Transformed = Operation.TransformDeleteInsert(op2, op1);

            str = ApplyOp(ApplyOp(str, op1), op2Transformed);
            str2 = ApplyOp(ApplyOp(str2, op2), op1Transformed);

            Assert.Equal(str, str2);
            Assert.Equal(result, str);
        }

        //Test insertion in a region that is being deleted
        {
            string str = "Some text";
            string str2 = str;
            string result = "S text";

            var op1 = Operation.CreateInsertOp(2, "Text");
            var op2 = Operation.CreateDeleteOp(1, "ome");

            var op1Transformed = Operation.TransformInsertDelete(op1, op2);
            var op2Transformed = Operation.TransformDeleteInsert(op2, op1);

            str = ApplyOp(ApplyOp(str, op1), op2Transformed);
            str2 = ApplyOp(ApplyOp(str2, op2), op1Transformed);

            Assert.Equal(str, str2);
            Assert.Equal(result, str);
        }
    }

    [Fact]
    public void DeleteDelete_Test()
    {
        //Test same deletion
        {
            string str = "Some text";
            string str2 = str;
            string result = "So text";

            var op1 = Operation.CreateDeleteOp(2, "me");
            var op2 = Operation.CreateDeleteOp(2, "me");

            var op1Transformed = Operation.TransformDeleteDelete(op1, op2);
            var op2Transformed = Operation.TransformDeleteDelete(op2, op1);

            str = ApplyOp(ApplyOp(str, op1), op2Transformed);
            str2 = ApplyOp(ApplyOp(str2, op2), op1Transformed);

            Assert.Equal(str, str2);
            Assert.Equal(result, str);
        }


        //Test op1 before op2
        {
            string str = "Some text";
            string str2 = str;
            string result = "S xt";

            var op1 = Operation.CreateDeleteOp(1, "ome");
            var op2 = Operation.CreateDeleteOp(5, "te");

            var op1Transformed = Operation.TransformDeleteDelete(op1, op2);
            var op2Transformed = Operation.TransformDeleteDelete(op2, op1);

            str = ApplyOp(ApplyOp(str, op1), op2Transformed);
            str2 = ApplyOp(ApplyOp(str2, op2), op1Transformed);

            Assert.Equal(str, str2);
            Assert.Equal(result, str);
        }

        //Test op1 after op2
        {
            string str = "Some text";
            string str2 = str;
            string result = "S xt";

            var op1 = Operation.CreateDeleteOp(5, "te");
            var op2 = Operation.CreateDeleteOp(1, "ome");

            var op1Transformed = Operation.TransformDeleteDelete(op1, op2);
            var op2Transformed = Operation.TransformDeleteDelete(op2, op1);

            str = ApplyOp(ApplyOp(str, op1), op2Transformed);
            str2 = ApplyOp(ApplyOp(str2, op2), op1Transformed);

            Assert.Equal(str, str2);
            Assert.Equal(result, str);
        }

        //Test op1 overlaping with op2 like this:
        // op1: [   ]
        // op2:   [   ]
        {
            string str = "Some text";
            string str2 = str;
            string result = "Sxt";

            var op1 = Operation.CreateDeleteOp(1, "ome ");
            var op2 = Operation.CreateDeleteOp(3, "e te");

            var op1Transformed = Operation.TransformDeleteDelete(op1, op2);
            var op2Transformed = Operation.TransformDeleteDelete(op2, op1);

            str = ApplyOp(ApplyOp(str, op1), op2Transformed);
            str2 = ApplyOp(ApplyOp(str2, op2), op1Transformed);

            Assert.Equal(str, str2);
            Assert.Equal(result, str);
        }

        //Test op1 overlaping with op2 like this:
        // op1:     [   ]
        // op2:   [   ]
        {
            string str = "Some text";
            string str2 = str;
            string result = "Sxt";

            var op1 = Operation.CreateDeleteOp(3, "e te");
            var op2 = Operation.CreateDeleteOp(1, "ome ");

            var op1Transformed = Operation.TransformDeleteDelete(op1, op2);
            var op2Transformed = Operation.TransformDeleteDelete(op2, op1);

            str = ApplyOp(ApplyOp(str, op1), op2Transformed);
            str2 = ApplyOp(ApplyOp(str2, op2), op1Transformed);

            Assert.Equal(str, str2);
            Assert.Equal(result, str);
        }

        //Test op1 overlaping with op2 like this:
        // op1:   [   ]
        // op2:  [     ]
        {
            string str = "Some text";
            string str2 = str;
            string result = "Soxt";

            var op1 = Operation.CreateDeleteOp(3, "e t");
            var op2 = Operation.CreateDeleteOp(2, "me te");

            var op1Transformed = Operation.TransformDeleteDelete(op1, op2);
            var op2Transformed = Operation.TransformDeleteDelete(op2, op1);

            str = ApplyOp(ApplyOp(str, op1), op2Transformed);
            str2 = ApplyOp(ApplyOp(str2, op2), op1Transformed);

            Assert.Equal(str, str2);
            Assert.Equal(result, str);
        }

        //Test op1 overlaping with op2 like this:
        // op1:  [     ]
        // op2:    [ ]
        {
            string str = "Some text";
            string str2 = str;
            string result = "Soxt";

            var op1 = Operation.CreateDeleteOp(2, "me te");
            var op2 = Operation.CreateDeleteOp(3, "e t");

            var op1Transformed = Operation.TransformDeleteDelete(op1, op2);
            var op2Transformed = Operation.TransformDeleteDelete(op2, op1);

            str = ApplyOp(ApplyOp(str, op1), op2Transformed);
            str2 = ApplyOp(ApplyOp(str2, op2), op1Transformed);

            Assert.Equal(str, str2);
            Assert.Equal(result, str);
        }
    }
}