using System.Threading;
using UnityEngine;
using TMPro;
using Yarn.Unity;
using Yarn.Markup;

    public class DialoguePresenter : DialoguePresenterBase
    {
        [SerializeField] bool autoAdvance = true;
        [SerializeField] float autoAdvanceDelay = 2.0f;
        
        [SerializeField] TMP_Text? lineText;
        
        public override YarnTask OnDialogueCompleteAsync()
        {
            return YarnTask.CompletedTask;
        }

        public override YarnTask OnDialogueStartedAsync()
        {
            return YarnTask.CompletedTask;
        }

        public override YarnTask<DialogueOption> RunOptionsAsync(DialogueOption[] dialogueOptions, CancellationToken cancellationToken)
        {
            return YarnTask<DialogueOption?>.FromResult(null);
        }

        public async override YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
        {
            
            MarkupParseResult text = line.TextWithoutCharacterName;
            lineText.text = text.Text;
            await YarnTask.Delay((int)(autoAdvanceDelay * 1000), token.NextLineToken).SuppressCancellationThrow();
            return;
        }
    }
