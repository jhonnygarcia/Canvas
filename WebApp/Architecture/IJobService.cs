using System;
using System.Linq.Expressions;

namespace WebApp.Architecture
{
    public interface IJobService
    {
        void EnqueueProcess(Expression<Action> action, int? progressId = null);
        int CreateProgress();
    }
}
