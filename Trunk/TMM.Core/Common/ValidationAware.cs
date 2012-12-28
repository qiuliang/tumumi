using System;
using System.Collections.Generic;

namespace TMM.Core
{
    [Serializable]
    public class ValidationAware
    {
        private ICollection<string> errors;
        private ICollection<string> messages;
        private ICollection<string> successes;

        public ICollection<string> Errors
        {
            get { return errors; }
        }

        public ICollection<string> Messages
        {
            get { return messages; }
        }

        public ICollection<string> Successes
        {
            get { return successes; }
        }

        public void AddError(string error)
        {
            if (errors == null)
            {
                errors = new List<string>();
            }
            errors.Add(error);
        }

        public void AddMessage(string message)
        {
            if (messages == null)
            {
                messages = new List<string>();
            }
            messages.Add(message);
        }

        public void AddSuccess(string success)
        {
            if (successes == null)
            {
                successes = new List<string>();
            }
            successes.Add(success);
        }

        public void ClearErrors()
        {
            if (errors != null)
            {
                errors.Clear();
            }
        }

        public void ClearMessages()
        {
            if (messages != null)
            {
                messages.Clear();
            }
        }

        public void ClearSuccesses()
        {
            if (successes != null)
            {
                successes.Clear();
            }
        }

        public void Clear()
        {
            ClearErrors();
            ClearMessages();
            ClearSuccesses();
        }

        public bool HasErrors
        {
            get { return errors != null && errors.Count > 0; }
        }

        public bool HasMessages
        {
            get { return messages != null && messages.Count > 0; }
        }

        public bool HasSuccesses
        {
            get { return successes != null && successes.Count > 0; }
        }
    }
}
