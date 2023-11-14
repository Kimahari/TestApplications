// See https://aka.ms/new-console-template for more information

namespace ILMapper; 
public record A {
    public int Id { get; set; }
    public required string Name { get; set; }
}
//IL_0000 nop	
//IL_0001	newobj	B..ctor
//IL_0006	dup	
//IL_0007	ldarg.0	
//IL_0008	callvirt	A.get_Id ()
//IL_000D callvirt	B.set_Id (Int32)
//IL_0012 nop	
//IL_0013	dup	
//IL_0014	ldarg.0	
//IL_0015	callvirt	A.get_Prop2 ()
//IL_001A callvirt	B.set_Prop2 (Int32)
//IL_001F nop	
//IL_0020	stloc.0	
//IL_0021	br.s	IL_0023
//IL_0023	ldloc.0	
//IL_0024	ret	