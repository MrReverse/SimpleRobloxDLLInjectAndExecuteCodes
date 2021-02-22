# SimpleRobloxDLLInjectAndExecuteCodes

How to use?

for inject:

DLLPipe.Inject();

for execute:

DLLPipe.Execute("script");

for checking inject status:

if(DLLPipe.namedpipeexists("pipename"))
{
MessageBox.Show("Injected Succesfully");
}
