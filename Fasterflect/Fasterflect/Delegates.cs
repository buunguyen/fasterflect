#region License
// Copyright 2009 Buu Nguyen (http://www.buunguyen.net/blog)
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at http://fasterflect.codeplex.com/
#endregion

namespace Fasterflect
{
    public delegate object StaticAttributeGetter();
    public delegate object AttributeGetter(object target);

    public delegate void StaticAttributeSetter(object value);
    public delegate void AttributeSetter(object target, object value);

    public delegate object StaticMethodInvoker(object[] parameters);
    public delegate object MethodInvoker(object target, object[] parameters);

    public delegate object ConstructorInvoker(object[] parameters);
}
