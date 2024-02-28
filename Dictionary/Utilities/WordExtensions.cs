using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Dictionary.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Dictionary.Utilities
{
    public static class WordExtensions
    {
        public static Word GetWord(this WordDbContext context, int id)
        {
            if (context.Words.Any(w => w.Id == id))
            {
                Word w = context.Words.Find(id);
                return w;
            }
            else
            {
                return null;
            }
        }

        public static async Task<OperationResult<Word>> AddWord(this WordDbContext context, string word, ILogger logger)
        {
            OperationResult<Word> or = new OperationResult<Word>();
            Word w = new Word(word);
            try
            {
                context.Words.Add(w);
                await context.SaveChangesAsync();
                or.Oks.Add(w);
            }
            catch (Exception e)
            {
                List<Word> wds = context.Words.Where(w => w.WordW.ToLower() == word.ToLower()).ToList();
                if (wds.Any())
                {
                    logger.LogError(e, "failed to add/{0}, word already in db with Id {1}", word, wds.First().Id);
                }
                else
                {
                    logger.LogError(e, "failed to add/{0}", word);
                }
                or.Conflicts.AddRange(wds);
            }
            return or;
        }

        public static async Task<OperationResult<Word>> AddWordBatch(this WordDbContext context, BatchWords words, ILogger logger)
        {
            bool changed = false;
            OperationResult<Word> or = new OperationResult<Word>();
            OperationResult<Word> orAdd = new OperationResult<Word>();
            foreach (var word in words.Words)
            {
                if (context.Words.Any(w => w.WordW.ToLower() == word.ToLower()))
                {
                    List<Word> wds = context.Words.Where(w => w.WordW.ToLower() == word.ToLower()).ToList();
                    logger.LogError("failed to add/{0}, word already in db with Id {1}", word, wds.First().Id);
                    or.Conflicts.AddRange(wds);
                }
                else
                {
                    Word w = new Word(word);
                    try
                    {
                        context.Words.Add(w);
                        orAdd.Oks.Add(w);
                        changed = true;
                    }
                    catch (Exception e)
                    {
                        or.UnknownErrors.Add(w);
                        logger.LogError(e, "failed to add/{0}", word);
                    }
                }
            }

            try
            {
                if(changed)
                {
                    await context.SaveChangesAsync();
                    or.Append(orAdd);
                }
            }
            catch (Exception e1)
            {
                logger.LogError(e1, "failed to batch add words '{0}', one or more words already in db.",
                    JsonConvert.SerializeObject(words));
                //foreach (var wd in checkedWords)
                //{
                //    or.Append(await context.AddWord(wd, logger));
                //}
            }

            return or;
        }

        public static async Task<bool> DeleteWord(this WordDbContext context, string word)
        {
            if (context.Words.Any(w => w.WordW.ToLower() == word.ToLower()))
            {
                List<Word> wds = context.Words.Where(w => w.WordW.ToLower() == word.ToLower()).ToList();
                context.Words.RemoveRange(wds);
                await context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
