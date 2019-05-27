﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Layouts;
using Microsoft.AspNetCore.Components.Routing;
using BlazorContextMenu.Services;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.RenderTree;

namespace BlazorContextMenu
{
    public class ContextMenuTrigger : ComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            /*
                <div onclick="@(MouseButtonTrigger == MouseButtonTrigger.Left || MouseButtonTrigger == MouseButtonTrigger.Both ? $"blazorContextMenu.OnContextMenu(event, '{MenuId.Replace("'","\\'")}'); " : "")"
                    oncontextmenu="@(MouseButtonTrigger == MouseButtonTrigger.Right || MouseButtonTrigger == MouseButtonTrigger.Both ? $"blazorContextMenu.OnContextMenu(event, '{MenuId.Replace("'","\\'")}');": "")" 
                    class="@CssClass"> @ChildContent
                </div>
             */

            builder.OpenElement(0, WrapperTag);
            if (MouseButtonTrigger == MouseButtonTrigger.Left || MouseButtonTrigger == MouseButtonTrigger.Both)
            {
                builder.AddAttribute(1, "onclick",
                    EventCallback.Factory.Create<UIMouseEventArgs>(this, $"blazorContextMenu.OnContextMenu(event, '{MenuId.Replace("'", "\\'")}');"));
            }

            if (MouseButtonTrigger == MouseButtonTrigger.Right || MouseButtonTrigger == MouseButtonTrigger.Both)
            {
                builder.AddAttribute(2, "oncontextmenu",
                    EventCallback.Factory.Create<UIMouseEventArgs>(this, $"blazorContextMenu.OnContextMenu(event, '{MenuId.Replace("'", "\\'")}');"));
            }

            builder.AddAttribute(3, "class", CssClass);
            builder.AddAttribute(4, "id", Id);
            builder.AddContent(5, ChildContent);
            builder.CloseElement();

        }

        [Inject] private IJSRuntime jsRuntime { get; set; }
        [Inject] private BlazorContextMenuHandler blazorContextMenuHandler { get; set; }


        /// <summary>
        /// The id of the <see cref="ContextMenuTrigger" /> wrapper element.
        /// </summary>
        [Parameter]
        public string Id { get; protected set; }

        /// <summary>
        /// The Id of the <see cref="ContextMenu" /> to open. This parameter is required.
        /// </summary>
        [Parameter]
        public string MenuId { get; protected set; }

        /// <summary>
        /// Additional css class for the trigger's wrapper element.
        /// </summary>
        [Parameter]
        public string CssClass { get; protected set; }

        /// <summary>
        /// The mouse button that triggers the menu.
        ///
        /// </summary>
        [Parameter]
        public MouseButtonTrigger MouseButtonTrigger { get; protected set; }

        /// <summary>
        /// The trigger's wrapper element tag (default: "div").
        /// </summary>
        [Parameter]
        public string WrapperTag { get; protected set; } = "div";

        [Parameter]
        protected RenderFragment ChildContent { get; set; }

        protected override void OnInit()
        {
            if (string.IsNullOrEmpty(MenuId))
            {
                throw new ArgumentNullException(nameof(MenuId));
            }
        }

        protected override async Task OnAfterRenderAsync()
        {
            if (!blazorContextMenuHandler.ReferencePassedToJs)
            {
                await jsRuntime.InvokeAsync<object>("blazorContextMenu.SetMenuHandlerReference", new DotNetObjectRef(blazorContextMenuHandler));
                blazorContextMenuHandler.ReferencePassedToJs = true;
            }
        }

    }
}
