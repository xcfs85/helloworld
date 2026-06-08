using Moq;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Messaging;
using Pindou.Domain.Entities.Messaging;
using Pindou.Infrastructure.Repositories;
using Pindou.Infrastructure.Services.Messaging;
using System.Linq.Expressions;

namespace Pindou.Tests.Services;

public class MessageServiceTests
{
    private readonly Mock<IRepository<Message>> _messageRepoMock;
    private readonly Mock<IRepository<MessageSetting>> _settingRepoMock;
    private readonly MessageService _messageService;

    public MessageServiceTests()
    {
        _messageRepoMock = new Mock<IRepository<Message>>();
        _settingRepoMock = new Mock<IRepository<MessageSetting>>();
        _messageService = new MessageService(_messageRepoMock.Object, _settingRepoMock.Object);
    }

    #region SendAsync Tests

    [Fact]
    public async Task SendAsync_ShouldSendMessage_WhenEnabled()
    {
        var setting = new MessageSetting { UserId = "u1", CommentEnabled = true };
        _settingRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<MessageSetting, bool>>>()))
            .ReturnsAsync(setting);
        _messageRepoMock.Setup(r => r.InsertAsync(It.IsAny<Message>())).ReturnsAsync("m1");

        await _messageService.SendAsync("u1", "comment", "标题", "内容");

        _messageRepoMock.Verify(r => r.InsertAsync(It.IsAny<Message>()), Times.Once);
    }

    [Fact]
    public async Task SendAsync_ShouldNotSend_WhenDisabled()
    {
        var setting = new MessageSetting { UserId = "u1", CommentEnabled = false };
        _settingRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<MessageSetting, bool>>>()))
            .ReturnsAsync(setting);

        await _messageService.SendAsync("u1", "comment", "标题", "内容");

        _messageRepoMock.Verify(r => r.InsertAsync(It.IsAny<Message>()), Times.Never);
    }

    [Fact]
    public async Task SendAsync_ShouldSend_WhenNoSetting()
    {
        _settingRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<MessageSetting, bool>>>()))
            .ReturnsAsync((MessageSetting?)null);
        _messageRepoMock.Setup(r => r.InsertAsync(It.IsAny<Message>())).ReturnsAsync("m1");

        await _messageService.SendAsync("u1", "comment", "标题", "内容");

        _messageRepoMock.Verify(r => r.InsertAsync(It.IsAny<Message>()), Times.Once);
    }

    #endregion

    #region GetMessagesAsync Tests

    [Fact]
    public async Task GetMessagesAsync_ShouldReturnPagedMessages()
    {
        var messages = new List<Message>
        {
            new Message { Id = "m1", UserId = "u1", Type = "comment", Title = "标题", Content = "内容", IsRead = false, CreateTime = DateTime.Now }
        };
        _messageRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<Expression<Func<Message, bool>>>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<Message, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((messages, 1));

        var result = await _messageService.GetMessagesAsync("u1", null, new PageRequest { Page = 1, Size = 10 });

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.List);
    }

    #endregion

    #region GetUnreadCountAsync Tests

    [Fact]
    public async Task GetUnreadCountAsync_ShouldReturnCount()
    {
        _messageRepoMock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<Message, bool>>>())).ReturnsAsync(5);

        var result = await _messageService.GetUnreadCountAsync("u1");

        Assert.Equal(5, result);
    }

    #endregion

    #region MarkReadAsync Tests

    [Fact]
    public async Task MarkReadAsync_ShouldMarkRead()
    {
        var msg = new Message { Id = "m1", UserId = "u1", IsRead = false };
        _messageRepoMock.Setup(r => r.GetByIdAsync("m1")).ReturnsAsync(msg);
        _messageRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Message>())).ReturnsAsync(true);

        var result = await _messageService.MarkReadAsync("u1", "m1");

        Assert.True(result);
        Assert.True(msg.IsRead);
    }

    [Fact]
    public async Task MarkReadAsync_ShouldThrow_WhenNotOwner()
    {
        var msg = new Message { Id = "m1", UserId = "u2", IsRead = false };
        _messageRepoMock.Setup(r => r.GetByIdAsync("m1")).ReturnsAsync(msg);

        var ex = await Assert.ThrowsAsync<BizException>(() => _messageService.MarkReadAsync("u1", "m1"));
        Assert.Contains("无权", ex.Message);
    }

    [Fact]
    public async Task MarkReadAsync_ShouldThrow_WhenNotFound()
    {
        _messageRepoMock.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((Message?)null);

        var ex = await Assert.ThrowsAsync<BizException>(() => _messageService.MarkReadAsync("u1", "nonexistent"));
        Assert.Contains("不存在", ex.Message);
    }

    #endregion

    #region MarkAllReadAsync Tests

    [Fact]
    public async Task MarkAllReadAsync_ShouldMarkAllRead()
    {
        var messages = new List<Message>
        {
            new Message { Id = "m1", UserId = "u1", IsRead = false },
            new Message { Id = "m2", UserId = "u1", IsRead = false }
        };
        _messageRepoMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<Message, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(messages);
        _messageRepoMock.Setup(r => r.UpdateRangeAsync(It.IsAny<List<Message>>())).ReturnsAsync(true);

        var result = await _messageService.MarkAllReadAsync("u1");

        Assert.True(result);
        Assert.True(messages.All(m => m.IsRead));
    }

    [Fact]
    public async Task MarkAllReadAsync_ShouldReturnTrue_WhenNoUnread()
    {
        _messageRepoMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<Message, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new List<Message>());

        var result = await _messageService.MarkAllReadAsync("u1");

        Assert.True(result);
    }

    #endregion

    #region GetSettingsAsync Tests

    [Fact]
    public async Task GetSettingsAsync_ShouldReturnDefault_WhenNoSetting()
    {
        _settingRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<MessageSetting, bool>>>()))
            .ReturnsAsync((MessageSetting?)null);
        _settingRepoMock.Setup(r => r.InsertAsync(It.IsAny<MessageSetting>())).ReturnsAsync("s1");

        var result = await _messageService.GetSettingsAsync("u1");

        Assert.NotNull(result);
        Assert.True(result.CommentEnabled);
        Assert.True(result.LikeEnabled);
    }

    [Fact]
    public async Task GetSettingsAsync_ShouldReturnExisting()
    {
        var setting = new MessageSetting { UserId = "u1", CommentEnabled = false, LikeEnabled = true };
        _settingRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<MessageSetting, bool>>>()))
            .ReturnsAsync(setting);

        var result = await _messageService.GetSettingsAsync("u1");

        Assert.NotNull(result);
        Assert.False(result.CommentEnabled);
        Assert.True(result.LikeEnabled);
    }

    #endregion

    #region UpdateSettingsAsync Tests

    [Fact]
    public async Task UpdateSettingsAsync_ShouldUpdate()
    {
        var setting = new MessageSetting { UserId = "u1", CommentEnabled = true, LikeEnabled = true };
        _settingRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<MessageSetting, bool>>>()))
            .ReturnsAsync(setting);
        _settingRepoMock.Setup(r => r.UpdateAsync(It.IsAny<MessageSetting>())).ReturnsAsync(true);

        var request = new UpdateMessageSettingRequest { CommentEnabled = false };
        var result = await _messageService.UpdateSettingsAsync("u1", request);

        Assert.NotNull(result);
        Assert.False(result.CommentEnabled);
    }

    [Fact]
    public async Task UpdateSettingsAsync_ShouldCreate_WhenNoSetting()
    {
        _settingRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<MessageSetting, bool>>>()))
            .ReturnsAsync((MessageSetting?)null);
        _settingRepoMock.Setup(r => r.InsertAsync(It.IsAny<MessageSetting>())).ReturnsAsync("s1");
        _settingRepoMock.Setup(r => r.UpdateAsync(It.IsAny<MessageSetting>())).ReturnsAsync(true);

        var request = new UpdateMessageSettingRequest { LikeEnabled = false };
        var result = await _messageService.UpdateSettingsAsync("u1", request);

        Assert.NotNull(result);
        Assert.False(result.LikeEnabled);
    }

    #endregion
}