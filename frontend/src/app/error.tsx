'use client';
export default function ErrorPage({ reset }: { reset: () => void }) { return <section className="container page-section"><div className="empty-state"><h1>Căn bếp đang tạm nghỉ một chút.</h1><p>Chưa tải được nội dung. Vui lòng thử lại sau ít phút.</p><button className="btn btn-dark" onClick={reset}>Thử lại</button></div></section>; }
