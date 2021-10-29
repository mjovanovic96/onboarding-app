#pragma checksum "C:\Users\mjovanovic\source\repos\FoodOrderApp\FrontendService\Views\Home\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "4def95659c51f40e62dddf1fa5f9669813ff9873"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Index), @"mvc.1.0.view", @"/Views/Home/Index.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\mjovanovic\source\repos\FoodOrderApp\FrontendService\Views\_ViewImports.cshtml"
using FrontendService;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\mjovanovic\source\repos\FoodOrderApp\FrontendService\Views\_ViewImports.cshtml"
using FrontendService.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"4def95659c51f40e62dddf1fa5f9669813ff9873", @"/Views/Home/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c79b619696c6abbae752e035f488496f5d9d44fa", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "C:\Users\mjovanovic\source\repos\FoodOrderApp\FrontendService\Views\Home\Index.cshtml"
  
    ViewData["Title"] = "Onboarding Application - Food Order";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<div ng-controller=""FoodMenuController"" ng-init=""refresh()"">
    <div class=""container-fluid"">
        <div class=""row"">
            <div class=""col-xs-8 col-xs-offset-2 text-center"">
                <h2>Onboarding Application - Food Order</h2>
            </div>
        </div>
       
        <hr/>

        <div ng-app=""FoodOrderApp"">
           <div ng-controller=""FoodMenuController"" ng-init=""refresh()"">
                <div class=""row"">
                    <div class=""col-sm-12 col-lg-4 col-md-6"" ng-repeat=""item in foodMenu.items"">
                        <div class=""food-item-wrapper"">
                            <div class=""image"">
                                <!-- <img src=""{{ item.ImageUrl }}"" alt="""">-->
                            </div>
                            <div class=""details"">
                                <div class=""title"">
                                    {{ item.Name }}
                                </div>
                                <div class=""desc");
            WriteLiteral(@"ription"">
                                    {{ item.Description }}
                                </div>
                                <div class=""price"">
                                    {{ item.Price | number: 2 }}
                                </div>
                            </div>
                            <div class=""cart-icon"">
                                <!-- <img src=""{{ item.ImageUrl }}"" ng-click=""addToCart(item)"" alt="""">-->
                            </div>
                        </div>
                    </div>
                </div>
              </div>
           </div>
        </div>
    </div>
</div>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
