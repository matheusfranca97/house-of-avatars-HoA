using Supabase.Gotrue.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TaskProcessor : MonoBehaviour
{
    [SerializeField] protected Text errorText;

    protected void Succeed(string message)
    {
        errorText.gameObject.SetActive(true);
        errorText.text = message;
        errorText.color = Color.white;
    }

    public void Fail(Exception ex)
    {
        Debug.LogException(ex);
        if (ex is GotrueException goTrueEx)
        {
            Debug.Log($"Reason: {goTrueEx.Reason.ToString()}");
            if (goTrueEx.InnerException != null)
            {
                Fail(goTrueEx.InnerException);
                return;
            }
            if (!errorText)
            { 
                throw goTrueEx;
            }
            Fail(goTrueEx.Reason.ToString() + $" Reason: {goTrueEx.Reason.ToString()}");
        }
        else if (ex is AggregateException aggregateEx)
        {
            Fail(aggregateEx.InnerException);
        }
        //else if (ex is FirebaseException firebaseEx)
        //{
        //    Fail((AuthError)firebaseEx.ErrorCode);
        //}
        else if (!errorText)
        {
            throw ex;
        }
        else
        {
            Fail(ex.ToString());
        }
    }
    //private void Fail(AuthError error) => Fail(error.ToString());
    protected void Fail(string customError)
    {
        Debug.Log($"Login failed with {customError} error");
        errorText.text = $"Login failed with {customError} error";
    }

    protected IEnumerator ProcessTasks(Task[] processors, Action postAction = null, Action failAction = null)
    {
        foreach (Task processor in processors)
        {
            yield return new WaitUntil(() => processor.IsCompleted);
            if (processor.IsFaulted || processor.IsCanceled)
            {
                Fail(processor.Exception);
                if (failAction != null) failAction();
                yield break;
            }
        }
        if (postAction != null) postAction();
    }
    protected IEnumerator ProcessTask(Task processor, Action postAction = null, Action failAction = null)
    {
        yield return new WaitUntil(() => processor.IsCompleted);
        if (processor.IsFaulted || processor.IsCanceled)
        {
            Fail(processor.Exception);
            if (failAction != null) failAction();
            yield break;
        }
        if (postAction != null) postAction();
    }
    protected IEnumerator ProcessTask<T>(Task<T> processor, Action<T> postAction = null, Action failAction = null)
    {
        yield return new WaitUntil(() => processor.IsCompleted);
        if (processor.IsFaulted || processor.IsCanceled)
        {
            Fail(processor.Exception);
            if (failAction != null) failAction();
            yield break;
        }
        if (postAction != null) postAction(processor.Result);
    }
}