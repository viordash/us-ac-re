using Microsoft.Extensions.DependencyInjection;

namespace UsAcRe.Core.Actions {
	public static class ActionsProviderExtension {
		public static IServiceCollection AddTestsActions(this IServiceCollection services) {
			services.AddTransient<ElementMatchAction, ElementMatchAction>()
					.AddTransient<KeybdAction, KeybdAction>()
					.AddTransient<MouseClickAction, MouseClickAction>()
					.AddTransient<MouseDragAction, MouseDragAction>()
					.AddTransient<TextTypingAction, TextTypingAction>()
					.AddTransient<ActionSet, ActionSet>()
					.AddTransient<DelayAction, DelayAction>();
			return services;
		}
	}
}
