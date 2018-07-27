using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
public class RegisterViewCommand : Controller
{
    public override void Execute(object data)
    {
        View view = data as View;
        RegisterView(view);
    }
}