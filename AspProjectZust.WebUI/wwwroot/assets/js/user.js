var notificationClick = true;

function SendFollow(id) {
    $.ajax({
        url: `/Home/SendFollow/${id}`,
        method: "GET",
        success: function (data) {
            GetAllUsers();
            SendFollowCall(id);
        }
    })
}

function UnFollowCall(id) {
    $.ajax({
        url: `/Home/UnFollowCall?id=${id}`,
        method: "GET",
        success: function (data) {
            GetAllUsers();
            SendFollowCall(id);
        }
    })
}

function ConfirmRequest(senderId, receiverId, requestId) {
    $.ajax({
        url: `/Home/ConfirmRequest?senderId=${senderId}&&requestId=${requestId}`,
        method: "GET",
        success: function (data) {
            SendFollowCall(receiverId);
            SendFollowCall(senderId);
            GetAllUsers();
        }
    })
}


function NotificationClick() {
    if (!notificationClick) {
        $.ajax({
            url: `/Home/DeleteNotification`,
            method: "GET",
            success: function (receiverId) {
                SendFollowCall(receiverId);
            }
        })
    }
    notificationClick = !notificationClick;
}

function NotificationGeneralFormOfInformation(receiverId, requestId) {
    $.ajax({
        url: `/Home/NotificationGeneralFormOfInformation?requestId=${requestId}`,
        method: "GET",
        success: function (data) {
            SendFollowCall(receiverId);
        }
    })
}

 chooseMedia(event) {
    event.preventDefault();

    const fileUpload = document.getElementById('file-upload');
    const mediaPreview = document.getElementById('media-preview');

    fileUpload.onchange = function (event) {
        const file = event.target.files[0];
        const reader = new FileReader();

        reader.onload = function (e) {
            let mediaElement;

            if (file.type.includes('video')) {
                mediaElement = document.createElement('video');
                mediaElement.setAttribute('controls', true);
                mediaElement.src = e.target.result;
                mediaPreview.innerHTML = '';
                mediaPreview.appendChild(mediaElement);
                mediaPreview.style.padding = "0px";
            } else if (file.type.includes('image')) {
                mediaElement = document.createElement('img');
                mediaElement.src = e.target.result;
                mediaPreview.innerHTML = '';
                mediaPreview.appendChild(mediaElement);
                mediaPreview.style.padding = "10px";
            }
            mediaPreview.style.border = "2px dashed var(--black-color)";
        };
        reader.readAsDataURL(file);
    };
    fileUpload.click();
}


function chooseVideo(event) {
    event.preventDefault();

    const fileUpload = document.getElementById('file-upload-video');
    const mediaPreview = document.getElementById('media-preview');

    fileUpload.onchange = function (event) {
        const file = event.target.files[0];
        const reader = new FileReader();

        reader.onload = function (e) {
            let mediaElement;

            if (file.type.includes('video')) {
                mediaElement = document.createElement('video');
                mediaElement.setAttribute('controls', true);
                mediaElement.src = e.target.result;
                mediaPreview.innerHTML = '';
                mediaPreview.appendChild(mediaElement);
                mediaPreview.style.padding = "0px";
            } else if (file.type.includes('image')) {
                mediaElement = document.createElement('img');
                mediaElement.src = e.target.result;
                mediaPreview.innerHTML = '';
                mediaPreview.appendChild(mediaElement);
                mediaPreview.style.padding = "10px";
            }
            mediaPreview.style.border = "2px dashed var(--black-color)";
        };
        reader.readAsDataURL(file);
    };
    fileUpload.click();
}

function DeleteRequest(id, senderId) {
    $.ajax({
        url: `/Home/DeleteRequest?requestId=${id}&&senderId=${senderId}`,
        method: "GET",

        success: function (data) {
            GetAllUsers();
            GetMyRequests();
            SendFollowCall(senderId);
        }
    })
}

function DeleteAllMessageNotifications() {
    $.ajax({
        url: `/Home/DeleteMessageNotification`,
        method: "GET",
        success: function (receiverId) {
            SendFollowCall(receiverId);
        }
    })
}

function GetMyRequests() {
    $.ajax({
        url: "/Home/GetAllRequests",
        method: "GET",
        success: function (data) {
            let content = "";
            let requestCount = 0;
            let notificationCount = 0;
            let userMessageNotificationCount = 0;
            let notificatonsContent = "";
            let notificationMessage = "";
            let subContent = "";
            for (var i = 0; i < data.length; i++) {
                if (data[i].status == "Request") {
                    content += `
                            <div class="item d-flex align-items-center">
                                <div class="figure">
                                    <a href="#"><img src="/assets/images/user/${data[i].sender.imageUrl}" class="rounded-circle" alt="image"></a>
                                </div>

                                <div class="content d-flex justify-content-between align-items-center">
                                    <div class="text">
                                        <h4><a href="#">${data[i].sender.userName}</a></h4>
                                    </div>
                                    <div class="btn-box d-flex align-items-center">
                                        <button class="delete-btn d-inline-block me-2" data-bs-toggle="tooltip" data-bs-placement="top" onclick="DeleteRequest(${data[i].id},'${data[i].senderId}')" title="Delete" type="button"><i class="ri-close-line"></i></button>

                                        <button class="confirm-btn d-inline-block" data-bs-toggle="tooltip" data-bs-placement="top" onclick="ConfirmRequest('${data[i].senderId}','${data[i].receiverId}','${data[i].id}')" title="Confirm" type="button"><i class="ri-check-line"></i></button>
                                    </div>
                                </div>
                            </div>
                           `;

                    subContent += `
                                <div class="item d-flex justify-content-between align-items-center">
                                    <div class="figure">
                                        <a href="#"><img src="/assets/images/user/${data[i].sender.imageUrl}" class="rounded-circle" alt="image"></a>
                                    </div>
                                    <div class="text">
                                        <h4><a href="#">${data[i].sender.userName}</a></h4>
                                        <span>${data[i].content}</span>
                                        <span class="main-color">${data[i].requestTime} Ago</span>
                                    </div>
                                </div>
                                `;

                    notificatonsContent += `
                    
                                       <div class="item d-flex justify-content-between align-items-center">
                                                       <div class="figure">
                                                           <a href="my-profile.html"><img src="/assets/images/user/${data[i].sender.imageUrl}" class="rounded-circle" alt="image"></a>
                                                       </div>
                                                       <div class="text">
                                                           <h4><a href="my-profile.html">${data[i].sender.userName}</a></h4>
                                                           <span>${data[i].content}</span>
                                                           <span class="main-color">${data[i].requestTime} Ago</span>
                                                       </div>
                                        </div> 
                    `;
                    requestCount += 1;
                    notificationCount += 1;
                }
                else if (data[i].status == "Message") {
                    notificationMessage += `
                                <div class="item d-flex justify-content-between align-items-center">
                                    <div class="figure">
                                        <a href="#"><img src="/assets/images/user/${data[i].sender.imageUrl}" width="50px" height="50px" class="rounded-circle" alt="image"></a>
                                    </div>
                                    <div class="text">
                                        <h4><a href='/Home/Messages/${data[i].sender.id}'>${data[i].sender.userName}</a></h4>
                                        <span>${data[i].content}</span>
                                    </div>
                                </div>
                    `;
                    userMessageNotificationCount += 1;
                }
                else {
                    subContent += `
                                <div class="item d-flex justify-content-between align-items-center">
                                    <div class="figure">
                                        <a href="#"><img src="/assets/images/user/${data[i].sender.imageUrl}" width="50px" height="50px" class="rounded-circle" alt="image"></a>
                                    </div>
                                    <div class="text">
                                        <h4><a href="#">${data[i].sender.userName}</a></h4>
                                        <span>${data[i].content}</span>
                                        <span class="main-color">${data[i].requestTime} Ago</span>
                                    </div>
                                </div>
                    `;

                    notificatonsContent += `
                                       <div class="item d-flex justify-content-between align-items-center">
                                                       <div class="figure">
                                                           <a href="my-profile.html"><img src="/assets/images/user/${data[i].sender.imageUrl}" class="rounded-circle" alt="image"></a>
                                                       </div>
                                                       <div class="text">
                                                           <h4><a href="my-profile.html">${data[i].sender.userName}</a></h4>
                                                           <span>${data[i].content}</span>
                                                           <span class="main-color">${data[i].requestTime} Ago</span>
                                                       </div>
                                                       <div class="icon">
                                                           <a onclick="NotificationGeneralFormOfInformation('${data[i].receiverId}','${data[i].id}')"><i class="flaticon-x-mark"></i></a>
                                                       </div>
                                        </div> 
                    `;
                    notificationCount += 1;
                }
            }

            notificationMessage += `           
              <div class="view-all-messages-btn">
                 <a onclick="DeleteAllMessageNotifications()" class="default-btn">View All Messages</a>
              </div>
            
            `;
            $("#requests").html(content);
            $("#messageNotifications").html(notificationMessage);
            $("#notifications").html(subContent);
            $("#yourNotifications").html(notificatonsContent);
            $("#userRequestCount").html(requestCount);
            $("#notificationCount").html(notificationCount);
            $("#userMessageNotificationCount").html(userMessageNotificationCount);
        }
    })
}

function UserRefresh() {
    $.ajax({
        url: `/Home/CurrentUser`,
        method: "GET",
        success: function (user) {
            $(".likeCount").html(user.likeCount);
            $(".followingCount").html(user.followingCount);
            $(".followersCount").html(user.followersCount);
        }
    })
}

function AlreadySent(id) {
    $.ajax({
        url: `/Home/AlreadySent?id=${id}`,
        method: "GET",
        success: function (user) {
            GetAllUsers();
            SendFollowCall(id);
        }
    })
}

function SendMessage(receiverId, senderId) {
    let content = "";
    let element = document.getElementById("message");
    let liveChatSendMessage = document.getElementById("liveChatSendMessage");
    if (element != null && element.value != "") {
        content = element.value;
    }
    else {
        content = liveChatSendMessage.value;
    }
    var object = {
        receiverId: receiverId,
        senderId: senderId,
        message: content
    };
    $.ajax({
        url: `/Home/AddMessage`,
        method: "POST",
        data: object,
        success: function (data) {
            GetMessageCall(receiverId, senderId);

            SendFollowCall(receiverId);
            SendFollowCall(senderId);

            GetMessageLiveChatCall(receiverId, senderId);
            GetMessageLiveChatCall(senderId, receiverId);


            if (element != null) {
                element.value = "";
            }

            if (liveChatSendMessage != null) {
                liveChatSendMessage.value = "";
            }
        }
    })
}

function GetMessages(receiverId, senderId) {
    $.ajax({
        url: `/Home/GetAllMessages?receiverId=${receiverId}&senderId=${senderId}`,
        method: "GET",
        success: function (data) {
            let content = "";
            let notificationMessage = "";

            for (var i = 0; i < data.chat.messages.length; i++) {
                if (data.chat.messages[i].senderId == data.currenUserId) {
                    content += `
                          <div class="chat chat-left">
                                <div class="chat-avatar">
                                    <a routerLink="/profile" class="d-inline-block">
                                        <img src="/assets/images/user/${data.senderImageUrl}" width="30" height="30" class="rounded-circle" alt="image">
                                    </a>
                                </div>

                                <div class="chat-body">
                                    <div class="chat-message">
                                        <p>${data.chat.messages[i].content}</p>
                                        <span class="time d-block">${data.chat.messages[i].dateTimeString}</span>
                                    </div>
                                </div>
                            </div>
                    `;


                }
                else {
                    content += `
                        <div class="chat">
                                <div class="chat-avatar">
                                    <a routerLink="/profile" class="d-inline-block">
                                        <img src="/assets/images/user/${data.receiverImageUrl}" width="30" height="30" class="rounded-circle" alt="image">
                                    </a>
                                </div>

                                <div class="chat-body">
                                    <div class="chat-message">
                                         <p>${data.chat.messages[i].content}</p>
                                        <span class="time d-block">${data.chat.messages[i].dateTimeString}</span>
                                    </div>
                                </div>
                            </div>
                    `;
                }
            }


            $("#messages").html(content);
        }
    })
}

function UserMessage(id) {
    $.ajax({
        url: `/Home/UserMessage?id=${id}`,
        method: "GET",

        success: function (model) {
            let context = ``;
            let content = "";
            context += `
                      <div class="live-chat-header d-flex justify-content-between align-items-center" width="30" height="30">
                          <div class="live-chat-info">
                              <a href="#"><img src="/assets/images/user/${model.receiverImageUrl}" class="rounded-circle" alt="image"></a>
                              <h3>
                                  <a href="#">${model.receiverName}</a>
                              </h3>
                          </div>
                      
                          <ul class="live-chat-right">
                          </ul>
                      </div>

                 <div class="chat-content" id="liveChatMessages">

                </div>
                       
            <div class="chat-list-footer" style="display:flex">
                    <input type="text" id="liveChatSendMessage" class="form-control" placeholder="Type your message...">

                    <button onclick="SendMessage('${model.receiverUserId}','${model.currenUserId}')" class="send-btn d-inline-block">Send</button>
            </div>
            `;



            for (var i = 0; i < model.chat.messages.length; i++) {
                if (model.chat.messages[i].senderId == model.currenUserId) {
                    content += `
                          <div class="chat chat-left">
                                <div class="chat-avatar">
                                    <a routerLink="/profile" class="d-inline-block">
                                        <img src="/assets/images/user/${model.senderImageUrl}" width="30" height="30" class="rounded-circle" alt="image">
                                    </a>
                                </div>

                                <div class="chat-body">
                                    <div class="chat-message">
                                        <p>${model.chat.messages[i].content}</p>
                                        <span class="time d-block">${model.chat.messages[i].dateTimeString}</span>
                                    </div>
                                </div>
                            </div>
                    `;
                }
                else {
                    content += `           
                          <div class="chat">
                                <div class="chat-avatar">
                                    <a routerLink="/profile" class="d-inline-block">
                                        <img src="/assets/images/user/${model.receiverImageUrl}" width="30" height="30" class="rounded-circle" alt="image">
                                    </a>
                                </div>

                                <div class="chat-body">
                                    <div class="chat-message">
                                        <p>${model.chat.messages[i].content}</p>
                                        <span class="time d-block">${model.chat.messages[i].dateTimeString}</span>
                                    </div>
                                </div>
                            </div>  
                    `;
                }
            }
            $("#liveChatBody").html(context);
            $("#liveChatMessages").html(content);
        }
    })
}

function AddPostComment(postId, userId) {
    var element = document.getElementById(`postComment${postId}`);
    $.ajax({
        url: `/Home/AddPostComment?postId=${postId}&commentAddedUserId=${userId}&coment=${element.value}`,
        method: "GET",
        success: function (data) {
            GetAllUsers();
            for (var i = 0; i < data.length; i++) {
                SendFollowCall(data[i].id);
            }
        }
    })
}

function Submit() {
    $.ajax({
        url: `/Home/AllUsers`,
        method: "GET",
        success: function (data) {
            for (var i = 0; i < data.length; i++) {
                SendFollowCall(data[i].id);
            }
        }
    })
}

function PostLike(postId) {
    $.ajax({
        url: `/Home/PostLike?postId=${postId}`,
        method: "Get",
        success: function (data) {
            GetAllUsers();
            for (var i = 0; i < data.length; i++) {
                SendFollowCall(data[i].id);
            }
        }
    })
}

function PostDisLike(postId) {
    $.ajax({
        url: `/Home/PostDisLike?postId=${postId}`,
        method: "Get",
        success: function (data) {
            GetAllUsers();
            for (var i = 0; i < data.length; i++) {
                SendFollowCall(data[i].id);
            }
        }
    })
}

function VideoClose() {
    $(".VideoEace").html("");
}

function VideoClick(videoUrl) {
    let content = `  
        <div class="mfp-bg mfp-fade mfp-ready" style="height: 1739px; position: absolute;"></div>

        <div class="mfp-wrap mfp-close-btn-in mfp-auto-cursor mfp-fade mfp-ready" tabindex="-1" style="top: 0px; position: absolute; height: 600px;">
            <div class="mfp-container mfp-iframe-holder">
                <div class="mfp-content">
                    <div class="mfp-iframe-scaler">
                        <button title="Close (Esc)" onclick="VideoClose()" type="button" class="mfp-close">×</button>
                        <iframe class="mfp-iframe" src="//www.youtube.com/embed/${videoUrl}?autoplay=1" frameborder="0" allowfullscreen="">
                         </iframe>
                     </div>
                </div>
            </div>
       </div>
    `;
    $(".VideoEace").html(content);
}

function DeletePost(postId) {
    $.ajax({
        url: `/Home/DeletePost?postId=${postId}`,
        method: "Get",
        success: function (data) {
            for (var i = 0; i < data.length; i++) {
                SendFollowCall(data[i].id);
            }
        }
    })
}

function GetMyAndFriendPosts() {
    $.ajax({
        url: `/Home/GetPosts`,
        method: "GET",
        success: function (data) {
            let content = "";
            for (var i = 0; i < data.friendPost.length; i++) {
                let comment = "";
                let postInYourComment = "";
                let userPostLike = "";
                let postVideo = "";
                let postImage = "";
                let message = "";
                let tagFriends = "";
                let deletePost = "";
                for (var k = 0; k < data.friendPost[i].comments.length; k++) {
                    comment += `
                             <div class="comment-list">
                                  <div class="comment-image">
                                      <a href="my-profile.html"><img src="/assets/images/user/${data.friendPost[i].comments[k].user.imageUrl}" class="rounded-circle" alt="image"></a>
                                  </div>
                                  <div class="comment-info">
                                      <h3>
                                          <a href="my-profile.html">${data.friendPost[i].comments[k].user.userName}</a>
                                      </h3>
                                      <span>${data.friendPost[i].comments[k].writeTime}</span>
                                      <p>${data.friendPost[i].comments[k].content}</p>
                                      <ul class="comment-react">

                                      </ul>
                                  </div>
                             </div>
                    `;
                }


                if (data.friendPost[i].comments.length > 3) {
                    comment += `
                    
                               <div class="more-comments">
                                  <a href="#">More Comments+</a>
                               </div>
                    
                    
                    `;
                }

                if (data.friendPost[i].customIdentityUserId == data.current.id) {
                    deletePost = `
                
                   <button class="dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i class="flaticon-menu"></i></button>
                   <ul class="dropdown-menu">
                       <li><a class="dropdown-item d-flex align-items-center deleteNotification" onclick="DeletePost(${data.friendPost[i].id})"><i class="flaticon-trash"></i> Delete Post</a></li>
                   </ul>
                
                `;
                }

                postInYourComment = `

                    <div>
                        <div class="footer-image">
                             <a href="#"><img style="width:100px;height:100px;" src="/assets/images/user/${data.current.imageUrl}" class="rounded-circle" alt="image"></a>
                         </div>
                         <div class="form-group" style="display:flex;">
                              <input id="postComment${data.friendPost[i].id}" class="form-control" placeholder="Write a comment..."></input>
                              <button style="border: 0px;color: darkblue;" onclick="AddPostComment(${data.friendPost[i].id},'${data.current.id}')" class="send-btn d-inline-block">
                           
                              <svg width="30px" height="20px" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
<path d="M11.5003 12H5.41872M5.24634 12.7972L4.24158 15.7986C3.69128 17.4424 3.41613 18.2643 3.61359 18.7704C3.78506 19.21 4.15335 19.5432 4.6078 19.6701C5.13111 19.8161 5.92151 19.4604 7.50231 18.7491L17.6367 14.1886C19.1797 13.4942 19.9512 13.1471 20.1896 12.6648C20.3968 12.2458 20.3968 11.7541 20.1896 11.3351C19.9512 10.8529 19.1797 10.5057 17.6367 9.81135L7.48483 5.24303C5.90879 4.53382 5.12078 4.17921 4.59799 4.32468C4.14397 4.45101 3.77572 4.78336 3.60365 5.22209C3.40551 5.72728 3.67772 6.54741 4.22215 8.18767L5.24829 11.2793C5.34179 11.561 5.38855 11.7019 5.407 11.8459C5.42338 11.9738 5.42321 12.1032 5.40651 12.231C5.38768 12.375 5.34057 12.5157 5.24634 12.7972Z" stroke="#000000" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
</svg>
                               </button>
 
                          </div>
                    
                    </div>
                    `

                if (data.friendPost[i].images != null) {
                    postImage += `
                            <img src="/assets/images/user/${data.friendPost[i].images}" alt="image"> 
                    `;
                }

                if (data.friendPost[i].videos != null) {
                    postVideo += ` 
                        <div class="single-video-card">
                             <div class="video-image">
                                <img src="/assets/images/video/DefaultVideoImage.png" alt="image">
                            
                                <div class="icon">
                                    <a onclick="VideoClick('${data.friendPost[i].videos}')" class="video-btn popup-youtube">
                                        <i class="flaticon-youtube"></i>
                                    </a>
                                </div>
                            </div>
                        </div>
                    `;
                }

                if (!data.friendPost[i].user.isUserLikedPost) {
                    userPostLike = `        
                       <a onclick="PostLike(${data.friendPost[i].id})"><i class="flaticon-like"></i><span>Like</span> <span class="number">${data.friendPost[i].likeCount} </span></a>  
                    `;
                }


                else {
                    userPostLike = `        
                       <a onclick="PostDisLike(${data.friendPost[i].id})"><i class="flaticon-like"></i><span>Dislike</span> <span class="number">${data.friendPost[i].likeCount} </span></a>  
                    `;
                }

                if (data.friendPost[i].content != null) {
                    message = `
                          <p>${data.friendPost[i].content}</p>
                    `;
                }

                content += `
                   <div class="news-feed news-feed-post">
                                <div class="post-header d-flex justify-content-between align-items-center">
                                    <div class="image">
                                        <a href="my-profile.html"><img src="/assets/images/user/${data.friendPost[i].user.imageUrl}" class="rounded-circle" alt="image"></a>
                                    </div>
                                    <div class="info ms-3">
                                        <span class="name"><a href="my-profile.html">${data.friendPost[i].user.userName}</a></span>
                                        <span class="small-text"><a href="#">${data.friendPost[i].publishTime} Ago</a></span>
                                    </div>
                                    <div class="dropdown">
                                        ${deletePost}
                                    </div>
                                </div>

                                <div class="post-body">
                                    ${message}

                                    <div class="post-image">
                                        ${postImage}
                                    </div>      

                                    ${postVideo}

                                    <ul class="post-meta-wrap d-flex justify-content-between align-items-center">
                                        <li class="post-react">
                                            ${userPostLike}

                                            
                                        </li>
                                        <li class="post-comment">
                                            <a href="#"><i class="flaticon-comment"></i><span>Comment</span> <span class="number">${data.friendPost[i].commentCount} </span></a>
                                        </li>
                                    </ul>
                                    <div class="post-comment-list" id="postInComments">

                                        ${comment}

                                    </div>

                                      

                                        ${postInYourComment}

                                </div>
                            </div>

                `;
            }

            content += ` 
                <div class="load-more-posts-btn">
                    <a href="#"><i class="flaticon-loading"></i> Load More Posts</a>
                </div>     
            `;
            $(".posts").html(content);
        }
    })
}

async function GetAllUsers() {
    $.ajax({
        url: "/Home/GetAllUsers",
        method: "GET",

        success: function (data) {
            var context = "";
            let subContent = "";
            let friendContent = "";
            let liveChatFriend = "";
            let d = "";
            for (var i = 0; i < data.length; i++) {
                if (data[i].isFriend) {
                    subContent = `<button class='btn btn-outline-secondary' onclick="UnFollowCall('${data[i].id}')">UnFollow</button>`;
                    if (data[i].isOnline) {
                        d += `
                    <div class="contact-item">
                        <a href="#"><img src="/assets/images/user/${data[i].imageUrl}" class="rounded-circle" alt="image"></a>
                        <span class="name"><a href="#">${data[i].userName}</a></span>
                        <span class="status-online"></span>
                    </div>
                    `;

                        liveChatFriend += `  
                            <div class="chat-box" style="width:60px;height:60px;margin-left:30px;">
                                       <div class="image">
                                           <a onclick="UserMessage('${data[i].id}')"><img src="/assets/images/user/${data[i].imageUrl}" class="rounded-circle" alt="image"></a>
                                           <span class="status-online"></span>
                                       </div>
                                 <h3>
                                     <a href="#">${data[i].userName}</a>
                                 </h3>
                            </div>     
                        `;
                    }
                    else {
                        d += `
                        <div class="contact-item">
                            <a href="#"><img src="/assets/images/user/${data[i].imageUrl}" class="rounded-circle" alt="image"></a>
                            <span class="name"><a href="#">${data[i].userName}</a></span>
                            <span class="status-offline"></span>
                        </div>
                        `;

                        liveChatFriend += `  
                                   <div class="chat-box" style="width:60px;height:60px;margin-left:30px;">
                                       <div class="image">
                                           <a onclick="UserMessage('${data[i].id}')"><img src="/assets/images/user/${data[i].imageUrl}" class="rounded-circle" alt="image"></a>
                                           <span class="status-offline"></span>
                                       </div>
                                       <h3>
                                           <a href="#">${data[i].userName}</a>
                                       </h3>
                                   </div>
                        `;
                    }

                    friendContent += `
                    
                        <div class="col-lg-3 col-sm-6">
                       <div class="single-friends-card">
                           <div class="friends-image">
                               <a href="#">
                                    <img src="/assets/images/friends/friends-bg-10.jpg" alt="image">
                               </a>
                               <div class="icon">
                                   <a href="#"><i class="flaticon-user"></i></a>
                               </div>
                           </div>
                           <div class="friends-content">
                               <div class="friends-info d-flex justify-content-between align-items-center">
                                   <a href="#">
                                        <img style="width:100px;height:100px" src='/assets/images/user/${data[i].imageUrl}' alt="image">
                                   </a>
                                   <div class="text ms-3">
                                       <h3><a href="#">${data[i].userName}</a></h3>
                                   </div>
                               </div>
                               <ul class="statistics">
                                   <li>
                                       <a href="#">
                                           <span class="item-number">${data[i].likeCount}</span>
                                           <span class="item-text">Likes</span>
                                       </a>
                                   </li>
                                   <li>
                                       <a href="#">
                                           <span class="item-number">${data[i].followingCount}</span>
                                           <span class="item-text">Following</span>
                                       </a>
                                   </li>
                                   <li>
                                       <a href="#">
                                           <span class="item-number">${data[i].followersCount}</span>
                                           <span class="item-text">Followers</span>
                                       </a>
                                   </li>
                               </ul>
                               <div class="button-group d-flex justify-content-between align-items-center">
                                   <div class="add-friend-btn">
                                   ${subContent}
                                   </div>
                                   <div class='send-message-btn'>
                                           <a href='/Home/Messages/${data[i].id}'>
                                             Send Message
                                           </a>
                                     </button>
                                   </div>
                               </div>
                           </div>
                       </div>
                   </div>
                    
                    
                    `;
                }
                else {
                    if (data[i].hasRequestPending) {
                        subContent = `<button onclick="AlreadySent('${data[i].id}')" class='btn btn-outline-secondary'>Already Sent</button>`;
                    }
                    else {
                        subContent = `<button onclick="SendFollow('${data[i].id}')" class='btn btn-outline-primary'>Follow</button>`;
                    }
                    context += `
                    <div class="col-lg-3 col-sm-6">
                       <div class="single-friends-card">
                           <div class="friends-image">
                               <a href="#">
                                    <img src="/assets/images/friends/friends-bg-10.jpg" alt="image">
                               </a>
                               <div class="icon">
                                   <a href="#"><i class="flaticon-user"></i></a>
                               </div>
                           </div>
                           <div class="friends-content">
                               <div class="friends-info d-flex justify-content-between align-items-center">
                                   <a href="#">
                                        <img style="width:100px;height:100px" src='/assets/images/user/${data[i].imageUrl}' alt="image">
                                   </a>
                                   <div class="text ms-3">
                                       <h3><a href="#">${data[i].userName}</a></h3>
                                   </div>
                               </div>
                               <ul class="statistics">
                                   <li>
                                       <a href="#">
                                           <span class="item-number">${data[i].likeCount}</span>
                                           <span class="item-text">Likes</span>
                                       </a>
                                   </li>
                                   <li>
                                       <a href="#">
                                           <span class="item-number">${data[i].followingCount}</span>
                                           <span class="item-text">Following</span>
                                       </a>
                                   </li>
                                   <li>
                                       <a href="#">
                                           <span class="item-number">${data[i].followersCount}</span>
                                           <span class="item-text">Followers</span>
                                       </a>
                                   </li>
                               </ul>
                               <div class="button-group d-flex justify-content-between align-items-center">
                                   <div class="add-friend-btn">
                                   ${subContent}
                                   </div>
                                   <div class="send-message-btn">
                                       <button type="submit">Send Message</button>
                                   </div>
                               </div>
                           </div>
                       </div>
                   </div>
                `;
                }
            }

            UserRefresh();
            GetMyAndFriendPosts();

            var id = document.getElementById("allUsers");
            if (id != null) {
                id.innerHTML = context;
            }

            var id2 = document.getElementById("onlineUsers");
            id2.innerHTML = d;
            var yourFriendElement = document.getElementById("yourFriend");

            var liveChatFriends = document.getElementById("liveChatFriends");
            if (liveChatFriends != null) {
                liveChatFriends.innerHTML = liveChatFriend;
            }


            if (yourFriendElement != null) {
                yourFriendElement.innerHTML = friendContent;
            }
        }
    })
}

GetMyRequests();