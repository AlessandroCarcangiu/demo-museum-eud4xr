# Prototype for Evaluating the EUD4XR Approach

To run the prototype, make sure the following components are up and running:

- The custom Home Assistant component developed in EUD4XR, which acts as the XR Rule Engine.
  - To configure it, set the Home Assistant server URL and token in `Assets/StreamingAssets/settings.json` (see the [official authentication documentation](https://www.home-assistant.io/docs/authentication/)).
- The server implementing the Intelligent Conversational Agent (ICA).
  - Set the chatbot URL in `Assets/EUD4XR_Chatbot/Scripts/Singletons/ChatbotManager.cs`.

Once everything is configured, connect the Meta headset to your PC and run the Unity application. To interact with the chatbot, use the left controller side trigger or say `Hey Bot`.