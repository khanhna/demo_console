wt -w 0 --title '2-tickle-api' --tabColor '#F5bbff' -d $pwd `
pwsh -noexit -c './0.be-common-variable.ps1 && ./0.be-running.ps1 $SourceDirectory TickleApi\src\Tickle.Application.Api $ProfileName'