using Unity.Netcode.Components;

namespace RMC.Playground2D.CA
{
	/// <summary>
	/// Used for syncing a animator with client side changes. 
	/// </summary>
	public class ClientNetworkAnimatorCA : NetworkAnimator
	{
		//  Properties ------------------------------------
		
		/// <summary>
		/// Used to determine who can write to this animator.
		/// </summary>
		protected override bool OnIsServerAuthoritative()
		{
			return false;
		}

		//  Unity Methods ---------------------------------
	}
}
