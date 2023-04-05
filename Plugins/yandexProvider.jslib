mergeInto(LibraryManager.library, {
  InitPurchases: function() {
    initPayments();
  },

  Purchase: function(id) {
    buy(id);
  },

  AuthenticateUser: function() {
    Auth();
  },

  playerSetData: function(data) {
    SetData(UTF8ToString(data));
  },

  playerGetData: function() {
    GetData();
  },

  playerSetLiderboard: function(lbName, value) {
    SetLiderboard(UTF8ToString(lbName), UTF8ToString(value));
  },

  GetUserData: function() {
    GetUserData();
  },

  ShowFullscreenAd: function () {
    ShowAd();
  },

  ShowRewardedAd: function(placement) {
    ShowRewarded(placement);
    return placement;
  },

  OpenWindow: function(link){
    var url = Pointer_stringify(link);
      document.onmouseup = function()
      {
        window.open(url);
        document.onmouseup = null;
      }
  }
});