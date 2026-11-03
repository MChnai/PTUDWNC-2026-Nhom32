import { WifiOff, ArrowUpRight } from 'lucide-react';
import Link from 'next/link';
export default function ServiceNotice() { return <div className="service-notice" role="alert"><WifiOff size={24}/><div><h3>Nội dung đang tạm gián đoạn</h3><p>Chưa tải được công thức. Bạn có thể thử lại sau hoặc ghé góc học nấu ăn trong lúc chờ.</p></div><Link className="text-link" href="/learn">Góc học nấu ăn <ArrowUpRight size={16}/></Link></div>; }
