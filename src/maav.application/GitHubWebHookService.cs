using MAAV.Application.Extensions;
using MAAV.DataContracts.GitHub;
using MAAV.Domain.Repositories;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MAAV.Application
{
    public class GitHubWebHookService : IGitHubWebHookService
    {
        private ConcurrentQueue<IGithubEvent> events = new ConcurrentQueue<IGithubEvent>();
        private bool disposed;
        private readonly Task longTask;
        private readonly IGithubEventResultRepository repository;
        private readonly IApplicationRepository appRepository;
        private CancellationTokenSource source = new CancellationTokenSource();


        public GitHubWebHookService(IGithubEventResultRepository repository, IApplicationRepository appRepository)
        {
            this.repository = repository;
            this.appRepository = appRepository;
            longTask = Task.Factory.StartNew(ExecutePersistencyEvent, TaskCreationOptions.LongRunning);
        }

        private void ExecutePersistencyEvent()
        {
            var token = source.Token;

            while (!disposed)
            {
                try
                {
                    Thread.Sleep(200);
                    if (!token.IsCancellationRequested && this.events.TryDequeue(out IGithubEvent @event))
                    {
                        var appLocated = this.appRepository.GetByAsync(app => @event.ApplicationId == app.Id && app.TeamCode == @event.TeamId && app.OrganisationId == @event.OrganisationId && app.WebHookEnabled).GetAwaiter().GetResult();
                        if (appLocated != null)
                        {
                            Domain.Entities.GithubEventResult eventResult = null;
                            if (@event is PushEvent)
                            {
                                var pushEvent = (@event as PushEvent);

                                eventResult = this.repository.GetByAsync(x => x.PushCommit == pushEvent.HeadCommit.Id).GetAwaiter().GetResult() ;

                                if (eventResult == null)
                                {

                                    eventResult = new Domain.Entities.GithubEventResult
                                    {
                                        FromBranch = "",
                                        ToBranch = pushEvent.Ref.Replace("refs/heads/", ""),
                                        PushCommit = pushEvent.HeadCommit.Id,
                                        CommitMessage = pushEvent.HeadCommit.Message,
                                        OrganisationId = appLocated.OrganisationId,
                                        TeamCode = appLocated.TeamCode,
                                        AppId = appLocated.Id,
                                    };


                                    if (!appLocated.KeyBranches.Any(b => Regex.IsMatch(eventResult.ToBranch, b.BranchPattern)))
                                    {
                                        continue;
                                    }

                                    this.repository.AddAsync(eventResult).GetAwaiter().GetResult();
                                }

                                eventResult.OrganisationId = appLocated.OrganisationId;
                                eventResult.TeamCode = appLocated.TeamCode;
                                eventResult.AppId = appLocated.Id;
                                eventResult.Status = "ready";
                            }
                            else
                            {
                                var pullRequestEvent = (@event as PullRequestEvent);
                                eventResult = this.repository.GetByAsync(x => x.PullRequestId == pullRequestEvent.PullRequest.Id).GetAwaiter().GetResult();

                                if (eventResult == null)
                                {
                                    eventResult = new Domain.Entities.GithubEventResult
                                    {
                                        FromBranch = pullRequestEvent.PullRequest.Head.Ref,
                                        ToBranch = pullRequestEvent.PullRequest.Base.Ref,
                                        PushCommit = pullRequestEvent.PullRequest.MergeCommitSha,
                                        CommitMessage = pullRequestEvent.PullRequest.Title + "|" + pullRequestEvent.PullRequest.Body,
                                        PullRequestId = pullRequestEvent.PullRequest.Id,
                                        OrganisationId = appLocated.OrganisationId,
                                        TeamCode = appLocated.TeamCode,
                                        AppId = appLocated.Id,
                                    };


                                    if (!appLocated.Branches.Any(b => Regex.IsMatch(eventResult.FromBranch, b.BranchPattern)))
                                    {
                                        if (!appLocated.KeyBranches.Any(b => Regex.IsMatch(eventResult.FromBranch, b.BranchPattern)))
                                        {
                                            continue;
                                        }
                                    }


                                    if (!appLocated.KeyBranches.Any(b => Regex.IsMatch(eventResult.ToBranch, b.BranchPattern)))
                                    {
                                        continue;
                                    }

                                    eventResult = this.repository.AddAsync(eventResult).GetAwaiter().GetResult();
                                }

                                eventResult.OrganisationId = appLocated.OrganisationId;
                                eventResult.TeamCode = appLocated.TeamCode;
                                eventResult.AppId = appLocated.Id;
                                eventResult.PullRequestId = pullRequestEvent.PullRequest.Id;
                                eventResult.FromBranch = pullRequestEvent.PullRequest.Head.Ref;
                                eventResult.ToBranch = pullRequestEvent.PullRequest.Base.Ref;
                                eventResult.PushCommit = pullRequestEvent.PullRequest.MergeCommitSha;
                                eventResult.CommitMessage = pullRequestEvent.PullRequest.Title + "|" + pullRequestEvent.PullRequest.Body;

                                eventResult.Status = pullRequestEvent.Action == "open" ? "pending" : pullRequestEvent.Action == "closed" && !pullRequestEvent.PullRequest.Merged ? "discard" : "pending";
                            }

                            this.repository.UpdateAsync(eventResult).GetAwaiter().GetResult();
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public Task EnqueueAsync(IGithubEvent @event)
        {
            if (!disposed)
            {
                this.events.Enqueue(@event);
            }

            return Task.CompletedTask;
        }


        private void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                this.disposed = true;
                this.events.Clear();
                this.source.Cancel();
                Task.WaitAll(longTask);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
