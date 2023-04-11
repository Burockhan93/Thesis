using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentQuestion 
{
    public enum Order { First,Second,Third,Fourth,Fifth}

    public Order _answerOrder;
    public string _question;
    public bool _isBetter;

    public ExperimentQuestion(string question)
    {
        this._question = question;
    }
   
}
